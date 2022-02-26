[UsesVerify]
public class ApplyScrubbersTests
{
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

        // Act
        ApplyScrubbers.UseAssembly(solutionDirectory, projectDirectory);
        ApplyScrubbers.Apply(extension, builder, settings);

        // Assert
        Assert.Equal("{SolutionDirectory},{ProjectDirectory}", builder.ToString());
    }
}