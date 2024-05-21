[TestClass]
public partial class NestedTypeTests
{
    [TestMethod]
    public Task ShouldPass() =>
        Verify("Foo");

    [TestClass]
    public partial class Nested
    {
        [TestMethod]
        public Task ShouldPass() =>
            Verify("NestedFoo");
    }
}