namespace VerifyXunit;

public static partial class Verifier
{
    public static SettingsTask VerifyDirectory(
        string path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyDirectory(path), true);

    public static SettingsTask VerifyDirectory(
        DirectoryInfo path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyDirectory(path), true);
}