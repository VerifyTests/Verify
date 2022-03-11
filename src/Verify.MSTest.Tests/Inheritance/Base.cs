namespace TheTests;

[TestClass]
public class Base :
    VerifyBase
{
    [TestMethod]
    public Task TestInBase() =>
        Verify("Foo");

    [TestMethod]
    public virtual Task TestToOverride() =>
        Verify("Foo");
}