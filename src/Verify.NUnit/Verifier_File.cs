namespace VerifyNUnit;

public static partial class Verifier
{
    public static SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path));

    public static SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path));
}