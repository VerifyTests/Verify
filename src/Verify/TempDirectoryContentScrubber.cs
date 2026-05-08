namespace VerifyTests;

sealed class TempDirectoryContentScrubber : ContentScrubber
{
    public override void Process(
        CharSpan input,
        StringBuilder output,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        output.Append(input);
        var paths = TempDirectory.AsyncPaths;
        if (paths == null)
        {
            return;
        }

        foreach (var path in paths)
        {
            output.Replace(path, "{TempDirectory}");
        }
    }
}
