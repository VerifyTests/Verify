using VerifyTests;

namespace VerifyExpecto
{
    public static partial class Verifier
    {
        static InnerVerifier GetVerifier(VerifySettings settings, string sourceFile)
        {
            if (CaptureName.TryGet(out var info))
            {
                return new(sourceFile, settings, null!);
            }

            var fileName = Path.GetFileName(sourceFile);
            throw new($"Expected to find a name. Ensure the following is called Runner.DefaultConfig.UseVerify(). File: {fileName}.");
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