[TestClass]
public partial class Base
{
    [TestMethod]
    public Task TestInBase() =>
        Verify("Foo");

    [TestMethod]
    public virtual Task TestToOverride() =>
        Verify("Foo");
}