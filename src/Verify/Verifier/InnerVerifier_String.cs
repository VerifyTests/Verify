partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyString(ValueTask<string> task) =>
        await VerifyString(await task);

    public async Task<VerifyResult> VerifyString(Task<string> task) =>
        await VerifyString(await task);

    public Task<VerifyResult> VerifyString(string? value) =>
        VerifyInner(value, null, emptyTargets, true);

    public async Task<VerifyResult> VerifyString(ValueTask<string> task, string extension) =>
        await VerifyString(await task, extension);

    public async Task<VerifyResult> VerifyString(Task<string> task, string extension) =>
        await VerifyString(await task, extension);

    public Task<VerifyResult> VerifyString(string? value, string extension) =>
        VerifyInner(
            new[]
            {
                new Target(extension, value ?? "null")
            });
}