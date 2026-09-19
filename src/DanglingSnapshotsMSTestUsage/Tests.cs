[TestClass]
public class Tests
{
    [TestMethod]
    public Task Simple() =>
        Verify("Foo");

    [TestMethod]
    public Task Second() =>
        Verify("Foo");
}