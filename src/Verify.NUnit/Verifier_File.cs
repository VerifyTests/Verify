using VerifyTests;

namespace VerifyNUnit;

public static partial class Verifier
{
    public static SettingsTask Verify(
        byte[] target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public static SettingsTask Verify(
        Task<byte[]> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(
            settings,
            sourceFile,
            async _ =>
            {
                var bytes = await target;
                await _.Verify(bytes);
            });
    }

    public static SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyFile(path));
    }

    public static SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyFile(path));
    }
}