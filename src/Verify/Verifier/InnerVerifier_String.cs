namespace VerifyTests;

partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyString(Task<string> task)
    {
        var value = await task;
        return await VerifyString(value);
    }

    public Task<VerifyResult> VerifyString(string? value) =>
        VerifyInner(value, null, emptyTargets, true);

    public async Task<VerifyResult> VerifyString(Task<string> task, string extension)
    {
        var value = await task;
        return await VerifyString(value, extension);
    }

    public Task<VerifyResult> VerifyString(string? value, string extension) =>
        VerifyInner(
            new[]
            {
                new Target(extension, value ?? "null")
            });
}