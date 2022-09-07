namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask VerifyDirectory(
        string path,
        Func<string, bool>? include = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyDirectory(path, include), true);

    public SettingsTask VerifyDirectory(
        DirectoryInfo path,
        Func<string, bool>? include = null,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyDirectory(path, include), true);
}