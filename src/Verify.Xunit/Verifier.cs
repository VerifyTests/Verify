using System;
using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using Xunit.Sdk;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile)
        {
            var className = Path.GetFileNameWithoutExtension(sourceFile);
            if (!UsesVerifyAttribute.TryGet(out var info))
            {
                throw new XunitException($"Expected to find a `[UsesVerify]` on `{className}`.");
            }

            var parameters = settings.GetParameters(info);

            var name = TestNameBuilder.GetUniqueTestName(className, info, parameters);
            return new InnerVerifier(name, sourceFile, info.DeclaringType!.Assembly, settings);
        }

        static SettingsTask Verify(VerifySettings? settings, string sourceFile, Func<InnerVerifier, Task> verify)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            return new SettingsTask(
                settings,
                async verifySettings =>
                {
                    using var verifier = GetVerifier(verifySettings, sourceFile);
                    await verify(verifier);
                });
        }
    }
}