[TestClass]
[UsesVerify]
public class Tests
{
    [TestMethod]
    public Task Simple() =>
        Verify("Foo");

    [TestMethod]
    public Task IncorrectCase() =>
        Verify("Foo");
}