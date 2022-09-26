﻿partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyString(Task<string> task)
    {
        var value = await task;
        return await VerifyString(value);
    }

    public Task<VerifyResult> VerifyString(string? value) =>
        VerifyInner(value, null, Enumerable.Empty<Target>());

    public async Task<VerifyResult> VerifyString(Task<string> task, string extension)
    {
        var value = await task;
        return await VerifyString(value, extension);
    }

    public Task<VerifyResult> VerifyString(string? value, string extension) =>
        VerifyInner(
            null,
            null,
            new[]
            {
                new Target(extension, value ?? "null")
            });
}