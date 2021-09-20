using VerifyTests;

namespace VerifyExpecto
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile)
        {
            if (CaptureName.TryGet(out var name))
            {
                return new(sourceFile, settings, uniqueness =>
                {
                    var directory = settings.Directory ?? Path.GetDirectoryName(sourceFile)!;
                    return ($"{name}_{uniqueness}", directory);
                });
            }

            var fileName = Path.GetFileName(sourceFile);
            throw new($"Expected to find a name. Ensure the following is called Runner.DefaultConfig.UseVerify(). File: {fileName}.");
        }

        static async Task Verify(VerifySettings? settings, string sourceFile, Func<InnerVerifier, Task> verify)
        {
            settings ??= new();
            Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
            using var verifier = GetVerifier(settings, sourceFile);
            await verify(verifier);
        }
    }
}