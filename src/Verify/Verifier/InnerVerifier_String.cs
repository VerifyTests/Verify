namespace VerifyTests;

partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyString([StringSyntax("*")] Task<string> task)
    {
        var value = await task;
        return await VerifyString(value);
    }

    public Task<VerifyResult> VerifyString([StringSyntax("*")] string? value) =>
        VerifyInner(value, null, emptyTargets, true, false);

    public async Task<VerifyResult> VerifyString([StringSyntax("*")] Task<string> task, string extension)
    {
        var value = await task;
        return await VerifyString(value, extension);
    }

    public Task<VerifyResult> VerifyString([StringSyntax("*")] string? value, string extension) =>
        VerifyInner(
        [
            new(extension, value ?? "null")
        ]);
}