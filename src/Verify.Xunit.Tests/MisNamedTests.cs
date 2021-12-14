public class NoAttributeTests
{
    [Fact]
    public async Task ShouldThrow()
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verify("Foo"));
        Assert.Equal("Expected to find a `[UsesVerify]` on test class. File: MisNamedTests.cs.", exception.Message);
    }

    public class Nested
    {
        [Fact]
        public async Task ShouldThrow()
        {
            var exception = await Assert.ThrowsAsync<Exception>(() => Verify("Foo"));
            Assert.Equal("Expected to find a `[UsesVerify]` on test class. File: MisNamedTests.cs.", exception.Message);
        }
    }
}