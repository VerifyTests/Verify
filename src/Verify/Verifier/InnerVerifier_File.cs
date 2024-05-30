namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyFile(string path, object? info, string? extension)
    {
        Guard.FileExists(path);
        if (extension == null)
        {
            return VerifyStream(IoHelpers.OpenRead(path), info);
        }

        return VerifyStream(IoHelpers.OpenRead(path), extension, info);
    }

    public Task<VerifyResult> VerifyFile(FileInfo target, object? info, string? extension) =>
        VerifyFile(target.FullName, info, extension);
}