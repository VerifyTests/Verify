using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Verify;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        public static Task Verify(
            string target,
            VerifySettings? settings = null,
            [CallerFilePath] string sourceFile = "")
        {
            var verifier = GetVerifier(sourceFile, settings);
            return verifier.Verify(target, settings);
        }
    }
}