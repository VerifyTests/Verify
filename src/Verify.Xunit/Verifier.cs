using System.IO;
using Verify;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(string sourceFile, string method, VerifySettings? settings)
        {
            var methodInfo = TypeCache.GetInfo(sourceFile, method);
            var parameters = settings.GetParameters(methodInfo);

            var className = Path.GetFileNameWithoutExtension(sourceFile);
            var name = TestNameBuilder.GetUniqueTestName(className, methodInfo, parameters);
            return new DisposableVerifier(name, sourceFile);
        }
    }
}