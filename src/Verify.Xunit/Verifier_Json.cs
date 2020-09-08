using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static Task Verify<T>(
            Task<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            var verifier = GetVerifier(settings);
            return verifier.Verify(target, settings);
        }

        public static Task Verify<T>(
            IAsyncEnumerable<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            var verifier = GetVerifier(settings);
            return verifier.Verify(target, settings);
        }

        public static Task Verify<T>(
            T target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            var verifier = GetVerifier(settings);
            return verifier.Verify(target, settings);
        }
    }
}