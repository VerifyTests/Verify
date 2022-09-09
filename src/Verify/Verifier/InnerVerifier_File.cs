partial class InnerVerifier
{
    public Task<VerifyResult> VerifyFile(string path)
    {
        Guard.FileExists(path);
        settings.extension ??= EmptyFiles.Extensions.GetExtension(path);
        return VerifyStream(IoHelpers.OpenRead(path));
    }

    public Task<VerifyResult> VerifyFile(FileInfo target) =>
        VerifyFile(target.FullName);
}