using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Verify;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(string sourceFile, string method, VerifySettings? settings)
        {
            var (type,methodInfo) = TypeCache.GetInfo(sourceFile, method);
            var settingsParameters = settings.GetParameters();
            var methodParameters = methodInfo.GetParameters();
            if (methodParameters.Any() && !settingsParameters.Any())
            {
                throw new Exception($@"Method `{type.Name}.{methodInfo.Name}` requires parameters, but none have been defined. Add UseParameters. For example:
var settings = new VerifySettings();
settings.UseParameters({string.Join(", ",methodParameters.Select(x=>x.Name))});
await Verifier.Verify(target, settings);");
            }

            var className = Path.GetFileNameWithoutExtension(sourceFile);
            var name = TestNameBuilder.GetUniqueTestName(className, methodInfo, settingsParameters);
            return new DisposableVerifier(type, Path.GetDirectoryName(sourceFile), name);
        }

        public static void SetTestAssembly(Assembly assembly)
        {
            Guard.AgainstNull(assembly, nameof(assembly));
            TypeCache.SetTestAssembly(assembly);
        }
    }
}