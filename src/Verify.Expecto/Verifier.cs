using VerifyTests;

namespace VerifyExpecto
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile, string name)
        {
            return new(
                sourceFile,
                settings,
                uniqueness =>
                {
                    var directory = settings.Directory ?? Path.GetDirectoryName(sourceFile)!;
                    var fileName = Path.GetFileNameWithoutExtension(sourceFile);
                    return ($"{fileName}.{name}{uniqueness}", directory);
                });
        }

        static async Task Verify(VerifySettings? settings, string sourceFile, string name, Func<InnerVerifier, Task> verify)
        {
            settings ??= new();
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            using var verifier = GetVerifier(settings, sourceFile, name);
            await verify(verifier);
        }
    }
}