using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static async Task Verify<T>(
            Task<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = GetVerifier(settings);
            await verifier.Verify(target, settings);
        }

        public static async Task Verify<T>(
            ValueTask<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = GetVerifier(settings);
            await verifier.Verify(target, settings);
        }

        public static async Task Verify<T>(
            IAsyncEnumerable<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = GetVerifier(settings);
            await verifier.Verify(target, settings);
        }

        public static async Task Verify<T>(
            T target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = GetVerifier(settings);
            await verifier.Verify(target, settings);
        }
    }
}