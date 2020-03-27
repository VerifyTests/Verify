using System.Diagnostics;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace TheNamesapce
{
    public class NamerInNamespaceTests :
        VerifyBase
    {
        [Fact]
        public Task Runtime()
        {
            var settings = new VerifySettings();
            settings.UniqueForRuntime();
            return Verify(Namer.Runtime, settings);
        }

        [Fact]
        public Task RuntimeAndVersion()
        {
            var settings = new VerifySettings();
            settings.UniqueForRuntimeAndVersion();
            return Verify(Namer.RuntimeAndVersion, settings);
        }

        [Fact]
        public void AccessNamerRuntimeAndVersion()
        {
            Debug.WriteLine(Namer.Runtime);
            Debug.WriteLine(Namer.RuntimeAndVersion);
        }

        [Fact]
        public Task AssemblyConfiguration()
        {
            var settings = new VerifySettings();
            settings.UniqueForAssemblyConfiguration();
            return Verify("Foo", settings);
        }

        public NamerInNamespaceTests(ITestOutputHelper output) :
            base(output)
        {
        }
    }
}