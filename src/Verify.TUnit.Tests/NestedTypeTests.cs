public class NestedTypeTests
{
    [Test]
    public Task ShouldPass() =>
        Verify("Foo");

    public class Nested
    {
        [Test]
        public Task ShouldPass() =>
            Verify("NestedFoo");
    }
}