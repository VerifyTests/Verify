using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        public static async Task Throws(
            Action target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(sourceFile);
            await verifier.Throws(target, settings);
        }

        public static async Task Throws(
            Func<object?> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(sourceFile);
            await verifier.Throws(target, settings);
        }

        public static async Task ThrowsAsync(
            Func<Task> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(sourceFile);
            await verifier.ThrowsAsync(target, settings);
        }

        public static async Task ThrowsAsync(
            Func<ValueTask> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(sourceFile);
            await verifier.ThrowsAsync(target, settings);
        }
    }
}