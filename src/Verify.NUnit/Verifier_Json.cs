using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Verify;

namespace VerifyNUnit
{
    public static partial class Verifier
    {
        public static async Task Verify<T>(
            Task<T> task,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile);
            await verifier.Verify(task, settings);
        }

        public static async Task Verify<T>(
            T target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            using var verifier = BuildVerifier(sourceFile);
            await verifier.Verify(target, settings);
        }
    }
}