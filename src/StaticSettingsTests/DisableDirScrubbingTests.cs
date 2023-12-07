[UsesVerify]
public class DisableDirScrubbingTests :
    BaseTest
{
    public DisableDirScrubbingTests()
    {
        VerifierSettings.DontScrubSolutionDirectory();
        VerifierSettings.DontScrubProjectDirectory();
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        VerifierSettings.AddScrubber(_ => _.Replace(solutionDirectory, "CustomReplace\\"));
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