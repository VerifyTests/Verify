public class DisableDirScrubbingTests :
    BaseTest
{
    public DisableDirScrubbingTests()
    {
        VerifierSettings.DontScrubSolutionDirectory();
        VerifierSettings.DontScrubProjectDirectory();
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        // ContentScrubber runs before pattern scrubbers, so this replacement is
        // applied to the buffer before DirectoryReplacementsPatternScrubber sees it.
        VerifierSettings.AddScrubber(new LambdaContentScrubber(_ => _.Replace(solutionDirectory, "CustomReplace\\")));
    }

    [Fact]
    public Task ProjectDirectory() =>
        Verify(AttributeReader
            .GetProjectDirectory()
            .TrimEnd('\\', '/'));

    [Fact]
    public Task SolutionDirectory() =>
        Verify(AttributeReader.GetSolutionDirectory());
}