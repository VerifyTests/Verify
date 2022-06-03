[UsesVerify]
public class Tests
{
    static Tests() =>
        VerifierSettings.AutoVerify();

    [Fact]
    public async Task Simple()
    {
        var path = Path.Combine(AttributeReader.GetProjectDirectory(), "Tests.Simple.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo");
        Assert.True(File.Exists(fullPath));
        File.Delete(fullPath);
    }
}