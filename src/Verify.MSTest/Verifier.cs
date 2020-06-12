using System;
using System.IO;
using System.Reflection;
using Verify;

namespace VerifyMSTest
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(string sourceFile, string method, VerifySettings? settings)
        {
            var methodInfo = GetMethodInfo(sourceFile, method, settings);
            var parameters = settings.GetParameters(methodInfo);

            var className = Path.GetFileNameWithoutExtension(sourceFile);
            var name = TestNameBuilder.GetUniqueTestName(className, methodInfo, parameters);
            return new InnerVerifier(name, sourceFile);
        }

        static MethodInfo GetMethodInfo(string sourceFile, string method, VerifySettings? settings)
        {
            if (settings.TryGetTestContext(out var context))
            {
                var properties = context!.Properties;
                if (properties.Contains("FullyQualifiedTestClassName"))
                {
                    var typeName = (string) properties["FullyQualifiedTestClassName"];
                    var type = Type.GetType(typeName);
                    return type.GetPublicMethod(method);
                }
            }

            return TypeCache.GetInfo(sourceFile, method);
        }
    }
}