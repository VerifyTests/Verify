[UsesVerify]
public class AutoVerify :
    BaseTest
{
    [Fact]
    public async Task Simple()
    {
        VerifierSettings.AutoVerify();
        var path = CurrentFile.Relative("AutoVerify.Simple.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo");
        File.Delete(fullPath);
    }
}