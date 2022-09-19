namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path));

    public SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "") =>
        Verify(settings, sourceFile, _ => _.VerifyFile(path));
}