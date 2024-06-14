[TestClass]
public class BaseClassTests : VerifyBase
{
    [TestMethod]
    public async Task VerifyFileTest() =>
        await VerifyFile(path: Path.GetFullPath("sample.png"));
}
