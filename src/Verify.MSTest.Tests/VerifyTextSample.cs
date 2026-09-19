[TestClass]
public class VerifyTextSample
{
    [TestMethod]
    public Task Simple() =>
        Verify("Foo");
}
