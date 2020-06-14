using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace TheNamespace.Bar
{
    [UsesVerify]
    public class NamerInNamespaceTests
    {
        [Fact]
        public Task Run()
        {
            return Verifier.Verify("value");
        }
    }
}