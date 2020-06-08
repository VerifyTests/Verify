using System.IO;
using System.Reflection;
using Verify;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(string sourceFile, string method, VerifySettings? settings)
        {
            var (type,methodInfo) = TypeCache.GetInfo(sourceFile, method);

            var className = Path.GetFileNameWithoutExtension(sourceFile);
            var name = TestNameBuilder.GetUniqueTestName(className, methodInfo, settings.GetParameters());
            return new DisposableVerifier(type, Path.GetDirectoryName(sourceFile), name);
        }

        public static void SetTestAssembly(Assembly assembly)
        {
            Guard.AgainstNull(assembly, nameof(assembly));
            TypeCache.SetTestAssembly(assembly);
        }
    }
}