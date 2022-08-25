#if DEBUG

[UsesVerify]
public class AttributeReaderTests
{
    [Fact]
    public Task Simple()
    {
        var assembly = typeof(AttributeReaderTests).Assembly;
        return Verify(
            new
            {
                TargetFrameworks = AttributeReader.GetTargetFrameworks(assembly),
                TargetFrameworksInferred = AttributeReader.GetTargetFrameworks(),
                ProjectDirectory = AttributeReader.GetProjectDirectory(assembly),
                ProjectDirectoryInferred = AttributeReader.GetProjectDirectory(),
                SolutionDirectory = AttributeReader.GetSolutionDirectory(assembly),
                SolutionDirectoryInferred = AttributeReader.GetSolutionDirectory()
            });
    }
}

#endif