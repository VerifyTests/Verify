[TestClass]
public class VerifyBaseTests : VerifyBase
{
    [TestMethod]
    public Task Simple() =>
        Verify("Foo");
}