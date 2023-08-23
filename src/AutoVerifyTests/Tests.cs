[UsesVerify]
public class Tests
{
    static Tests()
    {
        #region StaticAutoVerify

        VerifierSettings.AutoVerify();

        #endregion
    }

    [Fact]
    public async Task Simple()
    {
        var path = CurrentFile.Relative("Tests.Simple.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo");
        File.Delete(fullPath);
    }
}