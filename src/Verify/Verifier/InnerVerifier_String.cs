partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyString(Task<string> task)
    {
        var value = await task;
        return await VerifyString(value);
    }

    public Task<VerifyResult> VerifyString(string value)
    {
        value = OrEmpty(value);
        return VerifyInner(
            value,
            null,
            Array.Empty<Target>());
    }
    public async Task<VerifyResult> VerifyString(Task<string> task, string extension)
    {
        var value = await task;
        return await VerifyString(value, extension);
    }

    public Task<VerifyResult> VerifyString(string value, string extension)
    {
        value = OrEmpty(value);
        return VerifyInner(
            null,
            null,
            new[]
            {
                new Target(extension, value)
            });
    }

    static string OrEmpty(string value)
    {
        if (value.Length == 0)
        {
            return "emptyString";
        }

        return value;
    }
}