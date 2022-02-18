public class ApplyScrubbersTests
{
    [Theory]
    [InlineData("/",  "/",  "/,/")]
    [InlineData("//", "//", "/,/")]
    [InlineData("/solution/",  "/project/",   "/solution,/project")]
    [InlineData("/solution//", "/project//",  "/solution,/project")]
    public void Apply_Solution_and_Project_directory_Scrubbers_on_Separator_ending_Values(
        string solutionDirectory,
        string projectDirectory,
        string input)
    {
        // Arrange
        var extension = string.Empty;
        var builder   = new StringBuilder(input);
        var settings  = new VerifySettings();

        // Act
        ApplyScrubbers.UseAssembly(solutionDirectory, projectDirectory);
        ApplyScrubbers.Apply(extension, builder, settings);

        // Assert
        Assert.Equal("{SolutionDirectory},{ProjectDirectory}", builder.ToString());
    }
}
