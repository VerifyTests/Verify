public class NestedTypeTests
{
    [Fact]
    public Task ShouldPass() =>
        Verify("Foo");

    public class Nested
    {
        [Fact]
        public Task ShouldPass() =>
            Verify("NestedFoo");
    }
}