[TestClass]
public class BaseClassTests : VerifyBase
{
    [TestMethod]
    public async Task UseFileName()
    {
        var fullPath = Path.GetFullPath("sample.png");
        await VerifyFile(fullPath)
            .UseFileName("fileName");
    }
}
