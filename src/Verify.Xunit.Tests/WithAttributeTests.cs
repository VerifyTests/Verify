[UsesVerify]
public class WithAttributeTests
{
    [Fact]
    public Task ShouldPass()
    {
        return Verify("Foo");
    }

    [UsesVerify]
    public class Nested
    {
        [Fact]
        public Task ShouldPass()
        {
            return Verify("NestedFoo");
        }
    }
}