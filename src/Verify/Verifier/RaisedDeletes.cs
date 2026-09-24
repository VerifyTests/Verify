/// <summary>
/// The deletes a run raised for verified files no target produced, recorded so a later run that
/// verifies against one of those files again can withdraw its delete.
/// </summary>
/// <remarks>
/// A delete waits in the tray or the viewer for someone to accept it. Nothing withdrew it when its
/// file came back into use - a target that came back, or a snapshot that moved inline and then
/// back out when the switch was turned off - so accepting it removed a file a passing test depends
/// on, and the next run failed for a snapshot that had been fine.
/// <para>
/// Withdrawing on every verification would cost a round trip to the queue owner per verified file,
/// for every codebase, and on Windows a connect to a port nothing listens on waits out its timeout.
/// So the run that raises a delete records the file, and a verification that uses a recorded file
/// withdraws the delete and drops the record. With nothing recorded, that costs one directory check
/// per process.
/// </para>
/// <para>
/// Kept in the intermediate directory beside the received maps, so per configuration and target
/// framework: a delete one framework's run raised is withdrawn by the next run of that framework
/// that uses the file. A record whose file has gone is dropped when the records are read, and all
/// of them whenever obj is cleaned.
/// </para>
/// </remarks>
static class RaisedDeletes
{
    internal const string DirectoryName = "VerifyDelete";

    /// <summary>
    /// Swapped in tests. What reaches the tray or the viewer is otherwise only observable from them.
    /// </summary>
    internal static Func<string, Task> AddDelete = DiffRunner.AddDeleteAsync;

    /// <inheritdoc cref="AddDelete" />
    internal static Action<string> SettleDelete = DiffRunner.SettleDelete;

    // The file system decides when two spellings are one file, and the delete a tray holds is
    // keyed the same way
    static readonly StringComparer pathComparer =
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

    static Lock locker = new();

    // Verified file to the record naming it. Read from disk once per process, the first time it is
    // asked for, and kept current as this process raises and settles
    static volatile ConcurrentDictionary<string, string>? recorded;

    /// <summary>
    /// Raises a delete for a verified file no target produced, recording it first so it is never
    /// pending without a record.
    /// </summary>
    public static Task Raise(string file)
    {
        // Nothing is raised where DiffEngine is switched off, so there is nothing to withdraw later
        if (!DiffRunner.Disabled)
        {
            Record(file);
        }

        return AddDelete(file);
    }

    /// <summary>
    /// Called for every verified file a verification used, whatever the comparison found. A file in
    /// use is not stale, so a delete an earlier run raised for it is withdrawn.
    /// </summary>
    public static void SettleIfRaised(string file)
    {
        // A settle answers to the same switch a delete does. The record stays for a run that can
        // reach the owner, rather than being dropped with its delete still pending
        if (DiffRunner.Disabled)
        {
            return;
        }

        var records = Recorded();
        if (records.IsEmpty ||
            !records.TryRemove(file, out var record))
        {
            return;
        }

        SettleDelete(file);
        TryDelete(record);
    }

    static void Record(string file)
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
            // Named by the file as the file system compares it, so a re run overwrites the same
            // record whatever case the path arrived in
            var record = Path.Combine(directory, $"{Fnv1a.Hash(Fold(file))}.txt");
            File.WriteAllText(record, file);
            Recorded()[file] = record;
        }
        catch
        {
            // Only ever used to withdraw the delete later, so failing to write one must not change
            // the test outcome.
        }
    }

    static ConcurrentDictionary<string, string> Recorded()
    {
        var current = recorded;
        if (current is not null)
        {
            return current;
        }

        using (locker.EnterScope())
        {
            return recorded ??= Load();
        }
    }

    static ConcurrentDictionary<string, string> Load()
    {
        var records = new ConcurrentDictionary<string, string>(pathComparer);
        var intermediate = VerifierSettings.IntermediateDir;
        if (intermediate is null)
        {
            return records;
        }

        var directory = Path.Combine(intermediate, DirectoryName);
        if (!Directory.Exists(directory))
        {
            return records;
        }

        string[] files;
        try
        {
            files = Directory.GetFiles(directory, "*.txt");
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            return records;
        }

        foreach (var record in files)
        {
            string file;
            try
            {
                file = File.ReadAllText(record);
            }
            catch (Exception exception)
                when (exception is IOException or UnauthorizedAccessException)
            {
                continue;
            }

            // A file that has gone takes its delete with it: accepted, or removed some other way,
            // and a tray drops a delete whose file is missing. So there is nothing left for the
            // record to withdraw, and keeping it would only grow this directory for good
            if (file.Length == 0 ||
                !File.Exists(file))
            {
                TryDelete(record);
                continue;
            }

            records[file] = record;
        }

        return records;
    }

    static string Fold(string path) =>
        ReferenceEquals(pathComparer, StringComparer.OrdinalIgnoreCase)
            ? path.ToLowerInvariant()
            : path;

    static void TryDelete(string record)
    {
        try
        {
            File.Delete(record);
        }
        catch (Exception exception)
            when (exception is IOException or UnauthorizedAccessException)
        {
            // Settled either way. A record left behind is settled again by the next run that uses
            // the file, which finds nothing to withdraw.
        }
    }

    internal static void Reset() =>
        recorded = null;
}
