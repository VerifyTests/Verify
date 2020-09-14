using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify<T>(
            Task<T> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Verify(target, settings, sourceFile);
        }

        public Task Verify<T>(
            IAsyncEnumerable<T> target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Verify(target, settings, sourceFile);
        }

        public Task Verify<T>(
            T target,
            VerifySettings? settings = null)
        {
            settings ??= this.settings;
            return Verifier.Verify(target, settings, sourceFile);
        }
    }
}