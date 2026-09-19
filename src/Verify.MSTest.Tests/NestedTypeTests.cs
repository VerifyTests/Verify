[TestClass]
public class NestedTypeTests
{
    [TestMethod]
    public Task ShouldPass() =>
        Verify("Foo");

    [TestClass]
    public class Nested
    {
        [TestMethod]
        public Task ShouldPass() =>
            Verify("NestedFoo");
    }
}