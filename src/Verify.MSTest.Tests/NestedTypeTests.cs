[TestClass]
[UsesVerify]
public partial class NestedTypeTests
{
    [TestMethod]
    public Task ShouldPass() =>
        Verify("Foo");

    [TestClass]
    [UsesVerify]
    public partial class Nested
    {
        [TestMethod]
        public Task ShouldPass() =>
            Verify("NestedFoo");
    }
}