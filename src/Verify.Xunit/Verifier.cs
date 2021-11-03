using VerifyTests;

namespace VerifyXunit;

public static partial class Verifier
{
    static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile)
    {
        if (UsesVerifyAttribute.TryGet(out var info))
        {
            var type = info.ReflectedType!;
            TargetAssembly.Assign(type.Assembly);

            GetFileConvention fileConvention = uniqueness => ReflectionFileNameBuilder.FileNamePrefix(info, type, sourceFile, settings, uniqueness);

            return new(sourceFile, settings, fileConvention);
        }

        var fileName = Path.GetFileName(sourceFile);
        throw new($"Expected to find a `[UsesVerify]` on test class. File: {fileName}.");
    }

    static SettingsTask Verify(VerifySettings? settings, string sourceFile, Func<InnerVerifier, Task> verify)
    {
        Guard.AgainstBadSourceFile(sourceFile);
        return new(
            settings,
            async verifySettings =>
            {
                using var verifier = GetVerifier(verifySettings, sourceFile);
                await verify(verifier);
            });
    }
}