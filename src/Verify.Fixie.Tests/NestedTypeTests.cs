public class NestedTypeTests
{
    public Task ShouldPass() =>
        Verify("Foo");

    public class Nested
    {
        public Task ShouldPass() =>
            Verify("NestedFoo");
    }
}