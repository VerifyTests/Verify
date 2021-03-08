using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Verify.Xunit.Tests
{
    [UsesVerify]
    public class Issue307Repro
    {
        public class Element
        {
            public string? Id { get; set; }
        }

        [Fact]
        public async Task Repro()
        {
            var settings = new VerifySettings();
            settings.UseExtension("json");

            await Verifier
                .Verify(new Element(), settings);
        }
    }
}
