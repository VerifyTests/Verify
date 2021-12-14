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
                ProjectDirectory = AttributeReader.GetProjectDirectory(assembly),
                ProjectDirectoryInferred = AttributeReader.GetProjectDirectory(),
                SolutionDirectory = AttributeReader.GetSolutionDirectory(assembly),
                SolutionDirectoryInferred = AttributeReader.GetSolutionDirectory()
            });
    }
}