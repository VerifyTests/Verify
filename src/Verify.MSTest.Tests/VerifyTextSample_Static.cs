namespace TheTests;

[TestClass]
public class VerifyTextSample_Static
{
    [TestMethod]
    public Task Simple() =>
        Verify("Foo");
}