/// <summary>
/// The call sites the global inline switch handed over for review, recorded so a run with the
/// switch turned off can retire them.
/// </summary>
/// <remarks>
/// A snapshot the switch inlines has no Snapshot call yet, so the patch it hands over appends one.
/// Once the switch is off, nothing in the source says that call site was ever inline, and a
/// verification only retires where inline is in play - which is what spares a codebase that never
/// turned the switch on a round trip to the queue owner per verification. So the entry stayed
/// pending for good, in the queue or staged on disk, and accepting it appended a Snapshot call that
/// turned inline back on for that test, since an explicit Snapshot wins over the switch.
/// <para>
/// Asking the owner what it holds would find these too, but every run of every codebase would pay
/// for the asking: nothing owning the queue is the ordinary state of a machine with no tray, and on
/// Windows a connect to a port nothing listens on waits out its timeout rather than being refused.
/// So the run that hands a patch over records the call site, and the first verification of a run
/// with the switch off retires whatever was recorded. Where nothing was, that costs one directory
/// check per process.
/// </para>
/// <para>
/// Kept in the intermediate directory, which is per configuration and target framework, so a
/// switch that is on for one framework and off for another does not have the second retiring what
/// the first has just queued. Removed whenever obj is cleaned.
/// </para>
/// </remarks>
static class InlineSwitchRecords
{
    internal const string DirectoryName = "VerifyInlineSwitch";

    static Lock locker = new();
    static volatile bool retired;

    /// <summary>
    /// Records a call site whose snapshot is being handed over with a patch that appends a Snapshot
    /// call. Only the switch produces those, so only its call sites are recorded.
    /// </summary>
    public static void Write(string sourceFile, int line)
    {
        var intermediate = VerifierSettings.IntermediateDir;
        if (intermediate is null)
        {
            // The project does not consume Verify's build props, so the obj directory is unknown.
            return;
        }

        try
        {
            var directory = Path.Combine(intermediate, DirectoryName);
            Directory.CreateDirectory(directory);
            // Named by call site, so a re run overwrites the same record instead of accumulating
            var path = Path.Combine(directory, $"{Fnv1a.Hash($"{sourceFile}:{line}")}.txt");
            File.WriteAllText(path, $"{sourceFile}{Environment.NewLine}{line}");
        }
        catch
        {
            // Only ever used to clean up after the switch, so failing to write one must not change
            // the test outcome.
        }
    }

    /// <summary>
    /// Retires every recorded call site, once per process, when the switch is off. Called by every
    /// verification before it can queue, settle or retire anything of its own, so a record cannot
    /// retire an entry this run has just queued.
    /// </summary>
    public static void RetireIfSwitchedOff(VerifySettings settings)
    {
        if (retired ||
            // On, so whatever is recorded is still pending for a reason
            VerifierSettings.inline is not null ||
            // A retire tells the queue owner something, so it answers to the same switches a queue
            // does. Not marked done, so a later verification that can reach the owner still retires
            DiffRunner.Disabled ||
            !settings.diffEnabled ||
            InlineEngine.IsBuildServer())
        {
            return;
        }

        using (locker.EnterScope())
        {
            if (retired)
            {
                return;
            }

            RetireRecorded();
            retired = true;
        }
    }

    static void RetireRecorded()
    {
        var intermediate = VerifierSettings.IntermediateDir;
        if (intermediate is null)
        {
            return;
        }

        var directory = Path.Combine(intermediate, DirectoryName);
        if (!Directory.Exists(directory))
        {
            return;
        }

        foreach (var record in Records(directory))
        {
            if (TryRead(record, out var sourceFile, out var line))
            {
                InlineEngine.RetireRecorded(sourceFile, line);
            }

            TryDelete(record);
        }
    }

    static string[] Records(string directory)
    {
        try
        {
            return Directory.GetFiles(directory, "*.txt");
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    static bool TryRead(string record, [NotNullWhen(true)] out string? sourceFile, out int line)
    {
        sourceFile = null;
        line = 0;

        string[] lines;
        try
        {
            lines = File.ReadAllLines(record);
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            return false;
        }

        if (lines.Length < 2 ||
            lines[0].Length == 0 ||
            !int.TryParse(lines[1], out line) ||
            line < 1)
        {
            return false;
        }

        sourceFile = lines[0];
        return true;
    }

    static void TryDelete(string record)
    {
        try
        {
            File.Delete(record);
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            // A record left behind is retired again by a later run, which finds nothing to retire.
        }
    }

    internal static void Reset() =>
        retired = false;
}
