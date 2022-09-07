namespace TheTests;

[TestClass]
public class NestedTypeTests :
    VerifyBase
{
    [TestMethod]
    public Task ShouldPass() =>
        Verify("Foo");

    [TestClass]
    public class Nested :
        VerifyBase
    {
        [TestMethod]
        public Task ShouldPass() =>
            Verify("NestedFoo");
    }
}