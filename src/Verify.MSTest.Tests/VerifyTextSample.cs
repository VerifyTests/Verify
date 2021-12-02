namespace TheTests;

[TestClass]
public class VerifyTextSample :
    VerifyBase
{
    [TestMethod]
    public Task Simple()
    {
        return Verify("Foo");
    }
}