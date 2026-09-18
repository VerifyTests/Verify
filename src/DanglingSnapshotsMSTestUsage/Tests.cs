[TestClass]
public partial class Tests
{
    [TestMethod]
    public Task Simple() =>
        Verify("Foo");

    [TestMethod]
    public Task Second() =>
        Verify("Foo");
}