[TestClass]
[UsesVerify]
public partial class VerifyTextSample
{
    [TestMethod]
    public Task Simple() =>
        Verify("Foo");
}
