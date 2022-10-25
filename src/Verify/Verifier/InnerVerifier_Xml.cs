partial class InnerVerifier
{
    public Task<VerifyResult> VerifyXml(string? target)
    {
        if (target is null)
        {
            return VerifyInner(target, null, emptyTargets, true);
        }

        return VerifyXml(XDocument.Parse(target));
    }

    public async Task<VerifyResult> VerifyXml(Stream? target)
    {
        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, true);
        }

#if NET5_0_OR_GREATER
        var token = await XDocument.LoadAsync(target, LoadOptions.None, default);
#else
        var token = XDocument.Load(target, LoadOptions.None);
#endif
        return await VerifyXml(token);
    }

    public Task<VerifyResult> VerifyXml(XDocument target) =>
        VerifyInner(target, null, emptyTargets, true);
}