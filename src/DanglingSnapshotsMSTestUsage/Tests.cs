[TestClass]
[UsesVerify]
public partial class Tests
{
    [TestMethod]
    public Task Simple() =>
        Verify("Foo");

    [TestMethod]
    public Task IncorrectCase() =>
        Verify("Foo");
}