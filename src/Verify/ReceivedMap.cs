/// <summary>
/// Records which verified file a received file belongs to.
///
/// The verified name cannot be reliably derived from the received name. Multi targeted projects add
/// the runtime and version to the received name only, and ignored parameters are kept in the received
/// name but dropped from the verified name. In process consumers get both paths from
/// <see cref="FilePair" />, but out of process tooling (accept/review tools) only sees files on disk.
/// So the mapping is recorded for that tooling to read.
///
/// Maps are written to the intermediate (obj) directory rather than next to the snapshots, so that
/// they do not clutter the directory holding the code and snapshots, and so that they are not picked
/// up by the `*.received.*` globs tooling uses to find snapshots.
///
/// The file name is derived from the received path, so a re run overwrites the same map instead of
/// accumulating. Stale maps, for example from a deleted test, are never read, since tooling looks up
/// maps by the received files that actually exist. They are removed whenever obj is cleaned.
/// </summary>
static class ReceivedMap
{
    const string directoryName = "VerifyReceived";

    public static void Write(in FilePair file)
    {
        var intermediate = VerifierSettings.IntermediateDir;
        if (intermediate is null)
        {
            // The project does not consume Verify's build props, so the obj directory is unknown.
            return;
        }

        try
        {
            var directory = Path.Combine(intermediate, directoryName);
            Directory.CreateDirectory(directory);
            var path = Path.Combine(directory, $"{Hash(file.ReceivedPath)}.txt");
            File.WriteAllText(path, $"{file.ReceivedPath}{Environment.NewLine}{file.VerifiedPath}");
        }
        catch
        {
            // The map is an auxiliary artifact for tooling, so failing to write it must not change
            // the test outcome.
        }
    }

    // FNV-1a. Used instead of string.GetHashCode since that is randomized per process, and the name
    // has to be stable across runs for the map to be overwritten rather than duplicated.
    static string Hash(string value)
    {
        var hash = 14695981039346656037UL;
        foreach (var character in value)
        {
            hash ^= character;
            hash *= 1099511628211UL;
        }

        return hash.ToString("x16");
    }
}
