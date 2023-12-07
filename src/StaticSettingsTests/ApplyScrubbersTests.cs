[UsesVerify]
public class ApplyScrubbersTests :
    BaseTest
{
    public ApplyScrubbersTests() =>
        Scrubbers.ResetReplacements("FakeMachineName", "FakeUserName");

    [Theory]
    [InlineData("/", "/project", "/,/project")]
    [InlineData("/solution", "/project", "/solution,/project")]
    [InlineData("/solution/", "/project/", "/solution,/project")]
    [InlineData("/solution//", "/project//", "/solution,/project")]
    public void Apply_solution_and_project_directories_scrubbers(
        string solutionDirectory,
        string projectDirectory,
        string input)
    {
        // Arrange
        var extension = string.Empty;
        var builder = new StringBuilder(input);
        var settings = new VerifySettings();
        var counter = Counter.Start();
        // Act
        ApplyScrubbers.UseAssembly(solutionDirectory, projectDirectory);
        ApplyScrubbers.ApplyForExtension(extension, builder, settings, counter);
        Counter.Stop();
        // Assert
        Assert.Equal("{SolutionDirectory},{ProjectDirectory}", builder.ToString());
    }

    [Fact]
    public Task ScrubUserName() =>
        Verify("FakeUserName")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameValidBefore() =>
        Verify(" FakeUserName")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameValidAfter() =>
        Verify("FakeUserName ")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameValidWrapped() =>
        Verify(" FakeUserName ")
            .ScrubUserName();

    [Fact]
    public Task ScrubMachineName() =>
        Verify("FakeMachineName")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameValidBefore() =>
        Verify(" FakeMachineName")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameValidAfter() =>
        Verify("FakeMachineName ")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameValidWrapped() =>
        Verify(" FakeMachineName ")
            .ScrubMachineName();

    [Fact]
    public Task ScrubUserNameInValidBefore() =>
        Verify("AFakeUserName")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameInValidAfter() =>
        Verify("FakeUserNameA")
            .ScrubUserName();

    [Fact]
    public Task ScrubUserNameInValidWrapped() =>
        Verify("AFakeUserNameA")
            .ScrubUserName();

    [Fact]
    public Task ScrubMachineNameInValidBefore() =>
        Verify("AFakeMachineName")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameInValidAfter() =>
        Verify("FakeMachineNameA")
            .ScrubMachineName();

    [Fact]
    public Task ScrubMachineNameInValidWrapped() =>
        Verify("AFakeMachineNameA")
            .ScrubMachineName();
}