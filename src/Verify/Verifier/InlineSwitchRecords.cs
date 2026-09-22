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
/// A record lives only while its call site is still one the switch would append to. A later
/// switched-on verification that finds an explicit Snapshot there, or declines the call site,
/// forgets the record: an accepted Snapshot call is keyed by its own line, and a stale record on
/// that line would otherwise retire the explicit call's next pending snapshot.
/// </para>
/// <para>
/// Kept in the intermediate directory, which is per configuration and target framework, so a
/// framework whose switch was never on retires nothing another framework queued. The retire itself
/// names no framework, since the statement is that the call site is not inline any more: a
/// framework whose switch was on and is then turned off retires the whole entry, and a framework
/// still on re-queues on its next run. The directory survives a Clean target, which removes only
/// build outputs, but not an obj wipe: entries queued before that have no record and stay pending.
/// </para>
/// <para>
/// A project that does not consume Verify's build props has no intermediate directory, so nothing
/// is recorded and nothing is retired there; the same goes for a record whose write failed. And a
/// retire that reached nobody deletes its record all the same: an owner that has exited persisted
/// its queue to disk, where the staging clear finds it, and an owner that is running but did not
/// answer is the one case a record is lost for, since the send reports no outcome.
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
        if (RecordPath(sourceFile, line) is not { } path)
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, $"{sourceFile}{Environment.NewLine}{line}");
        }
        catch
        {
            // Only ever used to clean up after the switch, so failing to write one must not change
            // the test outcome.
        }
    }

    /// <summary>
    /// Drops the record for a call site the switch no longer appends to: an explicit Snapshot call
    /// is there now, or the switch declined it. Only called while the switch is on, so a codebase
    /// that never turned it on never pays the file check.
    /// </summary>
    public static void Forget(string sourceFile, int line)
    {
        if (RecordPath(sourceFile, line) is not { } path)
        {
            return;
        }

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            // Left for the switched-off run, which retires a call site the owner no longer holds.
        }
    }

    /// <summary>
    /// Named by call site, so a re run overwrites the same record instead of accumulating. Null
    /// when the project does not consume Verify's build props, so the obj directory is unknown.
    /// </summary>
    static string? RecordPath(string sourceFile, int line)
    {
        var intermediate = VerifierSettings.IntermediateDir;
        if (intermediate is null)
        {
            return null;
        }

        return Path.Combine(intermediate, DirectoryName, $"{Fnv1a.Hash($"{sourceFile}:{line}")}.txt");
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

            // The intermediate directory is assigned by the adapters, so a verification through the
            // raw InnerVerifier api can get here before it is known. Not marked done then either
            if (VerifierSettings.IntermediateDir is not { } intermediate)
            {
                return;
            }

            try
            {
                RetireRecorded(Path.Combine(intermediate, DirectoryName));
            }
            finally
            {
                // Whatever went wrong is paid once. Cleaning up after the switch must not change a
                // test outcome, and certainly not every test outcome in the process
                retired = true;
            }
        }
    }

    static void RetireRecorded(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        foreach (var record in Records(directory))
        {
            var read = TryRead(record, out var sourceFile, out var line);
            if (read == RecordRead.Unreadable)
            {
                // A transient failure to read is not a malformed record. Left for a later run,
                // since deleting it would strand the entry it names for good
                continue;
            }

            try
            {
                if (read == RecordRead.Read)
                {
                    InlineEngine.RetireRecorded(sourceFile!, line);
                }
            }
            catch (Exception exception)
                when (exception is not OutOfMemoryException)
            {
                // A record that cannot be retired never becomes retirable, so it is deleted below
                // rather than throwing into every verification of the process
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

    enum RecordRead
    {
        Read,
        Malformed,
        Unreadable
    }

    static RecordRead TryRead(string record, out string? sourceFile, out int line)
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
            return RecordRead.Unreadable;
        }

        if (lines.Length < 2 ||
            lines[0].Trim().Length == 0 ||
            !int.TryParse(lines[1], out line) ||
            line < 1)
        {
            return RecordRead.Malformed;
        }

        sourceFile = lines[0];
        return RecordRead.Read;
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
