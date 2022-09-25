[UsesVerify]
public class VerifyDirectoryTests
{
    static string directoryPathToVerify = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify");

    [Fact]
    public Task WithDirectory() =>
        VerifyDirectory(directoryPathToVerify);
}