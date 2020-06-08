using System;
using System.IO;
using System.Reflection;
using Verify;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static Assembly? testAssembly;

        static InnerVerifier GetVerifier(string sourceFile, string method, VerifySettings? settings)
        {
            if (testAssembly == null)
            {
                throw new Exception("Call `Verifier.SetTestAssembly(Assembly.GetExecutingAssembly());` at assembly startup.");
            }

            var className = Path.GetFileNameWithoutExtension(sourceFile);
            var type = testAssembly.GetType(className);

            if (type == null)
            {
                throw new Exception($"Type `{className}` not found in assembly `{testAssembly.GetName().Name}`.");
            }

            var methodInfo = type.GetMethod(method,BindingFlags.Instance|BindingFlags.Public|BindingFlags.FlattenHierarchy);
            if (methodInfo == null)
            {
                throw new Exception($"Method `{methodInfo}` not found on type `{type.Name}`.");
            }

            var name = TestNameBuilder.GetUniqueTestName(type, methodInfo, settings.GetParameters());
            return new DisposableVerifier(type, Path.GetDirectoryName(sourceFile), name);
        }

        public static void SetTestAssembly(Assembly assembly)
        {
            Guard.AgainstNull(assembly, nameof(assembly));
            testAssembly = assembly;
        }
    }
}