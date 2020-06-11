using System;
using System.IO;
using System.Linq;
using Verify;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(string sourceFile, string method, VerifySettings? settings)
        {
            var methodInfo = TypeCache.GetInfo(sourceFile, method);
            var settingsParameters = settings.GetParameters();
            var methodParameters = methodInfo.GetParameters();
            if (methodParameters.Any() && !settingsParameters.Any())
            {
                throw new Exception($@"Method `{methodInfo.DeclaringType.Name}.{methodInfo.Name}` requires parameters, but none have been defined. Add UseParameters. For example:
var settings = new VerifySettings();
settings.UseParameters({string.Join(", ",methodParameters.Select(x=>x.Name))});
await Verifier.Verify(target, settings);");
            }

            var className = Path.GetFileNameWithoutExtension(sourceFile);
            var name = TestNameBuilder.GetUniqueTestName(className, methodInfo, settingsParameters);
            return new DisposableVerifier(name, sourceFile);
        }
    }
}