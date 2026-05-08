namespace VerifyTests;

sealed class TempFileContentScrubber : ContentScrubber
{
    public override void Process(
        CharSpan input,
        StringBuilder output,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        output.Append(input);
        var paths = TempFile.AsyncPaths;
        if (paths == null)
        {
            return;
        }

        foreach (var path in paths)
        {
            output.Replace(path, "{TempFile}");
        }
    }
}
