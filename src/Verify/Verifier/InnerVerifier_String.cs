partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyString(Task<string> task)
    {
        var value = await task;
        return await VerifyString(value);
    }

    public Task<VerifyResult> VerifyString(string? value)
    {
        value = OrEmptyOrNull(value);
        return VerifyInner(value, null, Enumerable.Empty<Target>());
    }

    public async Task<VerifyResult> VerifyString(Task<string> task, string extension)
    {
        var value = await task;
        return await VerifyString(value, extension);
    }

    public Task<VerifyResult> VerifyString(string? value, string extension)
    {
        value = OrEmptyOrNull(value);
        return VerifyInner(
            null,
            null,
            new[]
            {
                new Target(extension, value)
            });
    }

    static string OrEmptyOrNull(string? value)
    {
        if (value is null)
        {
            return "null";
        }

        if (value.Length == 0)
        {
            return "emptyString";
        }

        return value;
    }
}