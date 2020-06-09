using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static Task Verify<T>(
            Task<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string methodName = "")
        {
            var verifier = GetVerifier(sourceFile, methodName, settings);
            return verifier.Verify(target, settings);
        }

        public static Task Verify<T>(
            IAsyncEnumerable<T> target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string methodName = "")
        {
            var verifier = GetVerifier(sourceFile, methodName, settings);
            return verifier.Verify(target, settings);
        }

        public static Task Verify<T>(
            T target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "",
            [CallerMemberName] string methodName = "")
        {
            var verifier = GetVerifier(sourceFile, methodName, settings);
            return verifier.Verify(target, settings);
        }
    }
}