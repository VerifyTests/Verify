using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Sdk;

public class NoAttributeTests
{
    [Fact]
    public async Task ShouldThrow()
    {
        var exception = await Assert.ThrowsAsync<XunitException>(() => Verifier.Verify("Foo"));
        Assert.Equal("Expected to find a `[UsesVerify]` on test class. File: MisNamedTests.cs.", exception.Message);
    }

    public class Nested
    {
        [Fact]
        public async Task ShouldThrow()
        {
            var exception = await Assert.ThrowsAsync<XunitException>(() => Verifier.Verify("Foo"));
            Assert.Equal("Expected to find a `[UsesVerify]` on test class. File: MisNamedTests.cs.", exception.Message);
        }
    }
}