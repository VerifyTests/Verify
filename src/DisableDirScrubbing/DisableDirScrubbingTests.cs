[UsesVerify]
public class DisableDirScrubbingTests
{
    static DisableDirScrubbingTests()
    {
        VerifierSettings.DontScrubSolutionDirectory();
        VerifierSettings.DontScrubProjectDirectory();
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        VerifierSettings.AddScrubber(s => s.Replace(solutionDirectory, "CustomReplace\\"));
    }

    [Fact]
    public Task ProjectDirectory() =>
        Verify(AttributeReader.GetProjectDirectory().TrimEnd('\\', '/'));

    [Fact]
    public Task SolutionDirectory() =>
        Verify(AttributeReader.GetSolutionDirectory());
}