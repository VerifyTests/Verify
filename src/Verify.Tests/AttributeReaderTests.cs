#if DEBUG

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
                ProjectName = AttributeReader.GetProjectName(assembly),
                ProjectNameInferred = AttributeReader.GetProjectName(),
                SolutionDirectory = AttributeReader.GetSolutionDirectory(assembly),
                SolutionDirectoryInferred = AttributeReader.GetSolutionDirectory(),
                SolutionName = AttributeReader.GetSolutionName(assembly),
                SolutionNameInferred = AttributeReader.GetSolutionName()
            });
    }
}

#endif