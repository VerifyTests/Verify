[TestClass]
public class VerifyBaseUsage :
    VerifyBase
{
    [TestMethod]
    public Task Simple() =>
        Verify("The content");
}