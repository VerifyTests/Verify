namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyFile(string path, object? info = null, string? extension = null)
    {
        Guard.FileExists(path);
        if (extension == null)
        {
            return VerifyStream(IoHelpers.OpenRead(path), info);
        }

        return VerifyStream(IoHelpers.OpenRead(path), extension, info);
    }

    public Task<VerifyResult> VerifyFile(FileInfo target, object? info = null, string? extension = null) =>
        VerifyFile(target.FullName, info, extension);

    public async Task<VerifyResult> VerifyFiles(
        IEnumerable<string> paths,
        object? info,
        FileScrubber? fileScrubber)
    {
        var targets = await ToTargetsForFiles(
            paths,
            info,
            fileScrubber);
        return await VerifyInner(targets);
    }
}