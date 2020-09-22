using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyMSTest
{
    public partial class VerifyBase
    {
        public async Task Throws(
            Action target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(settings);
            await verifier.Throws(target, settings);
        }

        public async Task Throws(
            Func<object> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(settings);
            await verifier.Throws(target, settings);
        }

        public async Task ThrowsAsync(
            Func<Task> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(settings);
            await verifier.ThrowsAsync(target, settings);
        }

        public async Task ThrowsAsync(
            Func<ValueTask> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            settings = settings.OrDefault(sourceFile);
            using var verifier = BuildVerifier(settings);
            await verifier.ThrowsAsync(target, settings);
        }
    }
}