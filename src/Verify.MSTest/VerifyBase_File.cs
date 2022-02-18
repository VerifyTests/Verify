namespace VerifyMSTest;

public partial class VerifyBase
{
    public SettingsTask Verify(
        byte[] target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.Verify(target));
    }

    public SettingsTask Verify(
        Task<byte[]> target,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(
            settings,
            sourceFile,
            async _ => { await _.Verify(await target); });
    }

    public SettingsTask VerifyFile(
        string path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyFile(path));
    }

    public  SettingsTask VerifyFile(
        FileInfo path,
        VerifySettings? settings = null,
        [CallerFilePath] string sourceFile = "")
    {
        return Verify(settings, sourceFile, _ => _.VerifyFile(path));
    }
}