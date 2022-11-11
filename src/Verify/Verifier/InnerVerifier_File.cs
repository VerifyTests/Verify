partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyFile(ValueTask<string> task, object? info) =>
        await VerifyFile(await task, info);

    public async Task<VerifyResult> VerifyFile(Task<string> task, object? info) =>
        await VerifyFile(await task, info);

    public Task<VerifyResult> VerifyFile(string path, object? info)
    {
        Guard.FileExists(path, nameof(path));
        return VerifyStream(IoHelpers.OpenRead(path), info);
    }

    public async Task<VerifyResult> VerifyFile(ValueTask<FileInfo> task, object? info) =>
        await VerifyFile(await task, info);

    public async Task<VerifyResult> VerifyFile(Task<FileInfo> task, object? info) =>
        await VerifyFile(await task, info);

    public Task<VerifyResult> VerifyFile(FileInfo target, object? info) =>
        VerifyFile(target.FullName, info);
}