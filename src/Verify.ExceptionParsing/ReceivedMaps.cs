using System.Runtime.InteropServices;

namespace VerifyTests.ExceptionParsing;

/// <summary>
/// Reads the received maps that Verify writes for out of process tooling.
///
/// The verified path cannot be reliably derived from the received path. A multi targeted project adds
/// the runtime and version to the received name only, and ignored parameters are kept in the received
/// name but dropped from the verified name. So, whenever Verify leaves a received file on disk, it
/// records which verified file that received file belongs to.
///
/// The records are written to the intermediate (obj) directory of the test project, in a
/// `VerifyReceived` directory, one file per received file, each holding the received path on the first
/// line and the verified path on the second.
/// </summary>
public sealed class ReceivedMaps
{
    // Kept in sync with the writer, ReceivedMap in Verify.
    const string directoryName = "VerifyReceived";

    static readonly StringComparer pathComparer =
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

    readonly Dictionary<string, string> verifiedByReceived;

    ReceivedMaps(IReadOnlyList<FilePair> pairs, Dictionary<string, string> verifiedByReceived)
    {
        Pairs = pairs;
        this.verifiedByReceived = verifiedByReceived;
    }

    /// <summary>
    /// Every received and verified pair that can be acted on, one per received file. Records whose
    /// received file no longer exists are excluded, as are duplicate records for the same received
    /// file, so this can be enumerated directly to accept a run.
    /// </summary>
    public IReadOnlyList<FilePair> Pairs { get; }

    /// <summary>
    /// Reads every received map under <paramref name="directory" />, recursively. Returns an empty
    /// instance if the directory does not exist or contains no maps.
    /// </summary>
    public static ReceivedMaps Read(string directory)
    {
        var lookup = new Dictionary<string, string>(pathComparer);

        foreach (var mapDirectory in FindMapDirectories(directory))
        {
            foreach (var file in EnumerateFiles(mapDirectory))
            {
                if (!TryReadPair(file, out var pair))
                {
                    continue;
                }

                var received = Normalize(pair.Received);

                // Records outlive the received files they describe, for example once a snapshot has
                // been accepted or the test has been fixed or deleted. Dropping those here keeps
                // every pair returned one that can actually be acted on.
                if (!File.Exists(received))
                {
                    continue;
                }

                // The same received path can be recorded by more than one build, for example Debug
                // and Release, with the same result. So the last wins rather than throwing.
                lookup[received] = pair.Verified;
            }
        }

        // Built from the lookup, so a received file recorded more than once yields a single pair,
        // and that pair agrees with what TryGetVerified returns.
        var pairs = new List<FilePair>(lookup.Count);
        foreach (var entry in lookup)
        {
            pairs.Add(new(entry.Key, entry.Value));
        }

        return new(pairs, lookup);
    }

    /// <summary>
    /// Finds the verified file that <paramref name="receivedPath" /> belongs to.
    /// </summary>
    /// <remarks>
    /// Records for a received file that no longer exists are never matched.
    /// </remarks>
    public bool TryGetVerified(string receivedPath, [NotNullWhen(true)] out string? verified) =>
        verifiedByReceived.TryGetValue(Normalize(receivedPath), out verified);

    // A junction or symlink can make the tree cyclic, and netstandard2.0 has no way to resolve a link
    // target to detect that. So the walk is bounded instead. An intermediate directory sits only a
    // handful of levels below the project or repository root it is scanned from.
    const int maxDepth = 20;

    static IEnumerable<string> FindMapDirectories(string directory)
    {
        if (!Directory.Exists(directory))
        {
            yield break;
        }

        var pending = new Stack<(string Directory, int Depth)>();
        pending.Push((directory, 0));

        while (pending.Count > 0)
        {
            var (current, depth) = pending.Pop();

            string[] children;
            try
            {
                children = Directory.GetDirectories(current);
            }
            catch (Exception exception)
                when (exception is UnauthorizedAccessException or IOException)
            {
                // Skip anything that cannot be walked, rather than failing the whole scan.
                continue;
            }

            foreach (var child in children)
            {
                var name = Path.GetFileName(child);
                if (string.Equals(name, directoryName, StringComparison.OrdinalIgnoreCase))
                {
                    // Maps are flat inside, so there is no need to descend further.
                    yield return child;
                    continue;
                }

                if (depth + 1 < maxDepth &&
                    !IsSkipped(name))
                {
                    pending.Push((child, depth + 1));
                }
            }
        }
    }

    // Directories that can never hold an intermediate directory, and that are large enough to dwarf
    // the rest of the walk when scanning from a repository root.
    static bool IsSkipped(string name) =>
        string.Equals(name, ".git", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(name, "node_modules", StringComparison.OrdinalIgnoreCase);

    static IEnumerable<string> EnumerateFiles(string directory)
    {
        try
        {
            return Directory.GetFiles(directory);
        }
        catch (Exception exception)
            when (exception is UnauthorizedAccessException or IOException)
        {
            return [];
        }
    }

    static bool TryReadPair(string file, out FilePair pair)
    {
        string[] lines;
        try
        {
            lines = File.ReadAllLines(file);
        }
        catch (Exception exception)
            when (exception is UnauthorizedAccessException or IOException)
        {
            pair = default;
            return false;
        }

        // Only the first two lines are read, so that a future version adding more does not break this.
        if (lines.Length < 2 ||
            lines[0].Length == 0 ||
            lines[1].Length == 0)
        {
            pair = default;
            return false;
        }

        pair = new(lines[0], lines[1]);
        return true;
    }

    static string Normalize(string path)
    {
        try
        {
            return Path.GetFullPath(path);
        }
        catch
        {
            // Not a usable path, so compare it as written.
            return path;
        }
    }
}
