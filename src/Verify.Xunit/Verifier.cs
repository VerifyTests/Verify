using System;
using System.IO;
using System.Threading.Tasks;
using VerifyTests;

namespace VerifyXunit
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile)
        {
            if (!UsesVerifyAttribute.TryGet(out var info))
            {
                var fileName = Path.GetFileName(sourceFile);
                throw new Exception($"Expected to find a `[UsesVerify]` on test class. File: {fileName}.");
            }

            var parameters = settings.GetParameters(info);

            var type = info.ReflectedType!;
            return new(sourceFile, type.Assembly, settings, info, parameters);
        }

        static SettingsTask Verify(VerifySettings? settings, string sourceFile, Func<InnerVerifier, Task> verify)
        {
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            return new(
                settings,
                async verifySettings =>
                {
                    using var verifier = GetVerifier(verifySettings, sourceFile);
                    await verify(verifier);
                });
        }
    }
}