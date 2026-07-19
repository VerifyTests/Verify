namespace VerifyTests;

// The scrubber shared by TempDirectory and TempFile. Each tracks the paths it has
// created for the current async context, so the paths are only known at scrub time
// and cannot be expressed as fixed Replace pairs.
static class TempPathScrubber
{
    // Replaces any tracked path with token. The leftmost match in the segment wins,
    // matching the engine's leftmost first scan.
    // All tracked paths start with the root directory, so segments shorter than it
    // are skipped without invoking the matcher.
    public static Scrubber Build(
        AsyncLocal<List<string>?> paths,
        object pathsLock,
        string token,
        int rootDirectoryLength) =>
        Scrubber.Match(
            (CharSpan segment, Counter _, IReadOnlyDictionary<string, object> _, out int index, out int length, out string? replacement) =>
            {
                index = -1;
                length = 0;
                replacement = token;
                var pathsValue = paths.Value;
                if (pathsValue == null)
                {
                    return false;
                }

                lock (pathsLock)
                {
                    foreach (var path in pathsValue)
                    {
                        var found = segment.IndexOf(path.AsSpan(), StringComparison.Ordinal);
                        if (found < 0)
                        {
                            continue;
                        }

                        if (index < 0 ||
                            found < index)
                        {
                            index = found;
                            length = path.Length;
                        }
                    }
                }

                return index >= 0;
            },
            minLength: rootDirectoryLength);
}
