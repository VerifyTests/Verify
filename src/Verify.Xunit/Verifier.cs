using System.IO;
using Verify;
using Xunit.Sdk;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(string sourceFile, VerifySettings? settings)
        {
            var className = Path.GetFileNameWithoutExtension(sourceFile);
            if (!InjectInfoAttribute.TryGet(out var info))
            {
                throw new XunitException($"Expected to find a `[{nameof(InjectInfoAttribute)}]` on `{className}`.");
            }

            var parameters = settings.GetParameters(info!);

            var name = TestNameBuilder.GetUniqueTestName(className, info!, parameters);
            return new InnerVerifier(name, sourceFile, info!.DeclaringType.Assembly);
        }
    }
}