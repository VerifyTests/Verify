namespace VerifyNUnit;

public static partial class Verifier
{
    public static SettingsTask VerifyDirectory(
        string path,
        Func<string, bool>? include = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyDirectory(path, include), true);

    public static SettingsTask VerifyDirectory(
        DirectoryInfo path,
        Func<string, bool>? include = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyDirectory(path, include), true);
}