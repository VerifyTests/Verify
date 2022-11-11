partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyJson(ValueTask<string?> task) =>
        await VerifyJson(await task);

    public async Task<VerifyResult> VerifyJson(Task<string?> task) =>
        await VerifyJson(await task);

    public Task<VerifyResult> VerifyJson(string? target)
    {
        if (target is null)
        {
            return VerifyInner(target, null, emptyTargets, true);
        }

        return VerifyJson(JToken.Parse(target));
    }

    public async Task<VerifyResult> VerifyJson(ValueTask<Stream?> task) =>
        await VerifyJson(await task);

    public async Task<VerifyResult> VerifyJson(Task<Stream?> task) =>
        await VerifyJson(await task);

    public async Task<VerifyResult> VerifyJson(Stream? target)
    {
        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, true);
        }

        using var reader = new StreamReader(target);
        using var textReader = new JsonTextReader(reader);
        var token = await JToken.LoadAsync(textReader);
        return await VerifyJson(token);
    }

    public Task<VerifyResult> VerifyJson(JToken target) =>
        VerifyInner(target, null, emptyTargets, true);
}