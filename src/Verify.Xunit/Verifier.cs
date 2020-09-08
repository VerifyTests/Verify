using System.IO;
using VerifyTests;
using Xunit.Sdk;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(VerifySettings settings)
        {
            var className = Path.GetFileNameWithoutExtension(settings.SourceFile);
            if (!UsesVerifyAttribute.TryGet(out var info))
            {
                throw new XunitException($"Expected to find a `[UsesVerify]` on `{className}`.");
            }

            var parameters = settings.GetParameters(info);

            var name = TestNameBuilder.GetUniqueTestName(className, info, parameters);
            return new InnerVerifier(name, settings.SourceFile, info.DeclaringType!.Assembly);
        }
    }
}