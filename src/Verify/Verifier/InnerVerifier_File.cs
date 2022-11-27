namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyFile(string path, object? info)
    {
        Guard.FileExists(path, nameof(path));
        return VerifyStream(IoHelpers.OpenRead(path), info);
    }

    public Task<VerifyResult> VerifyFile(FileInfo target, object? info) =>
        VerifyFile(target.FullName, info);
}