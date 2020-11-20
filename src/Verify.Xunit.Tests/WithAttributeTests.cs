using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class WithAttributeTests
{
    [Fact]
    public Task ShouldPass()
    {
        return Verifier.Verify("Foo");
    }

    [UsesVerify]
    public class Nested
    {
        [Fact]
        public Task ShouldPass()
        {
            return Verifier.Verify("NestedFoo");
        }
    }
}