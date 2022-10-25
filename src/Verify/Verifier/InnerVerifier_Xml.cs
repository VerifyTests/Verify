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

    public Task<VerifyResult> VerifyXml(XDocument target)
    {
        var serialization = settings.serialization;
        var xElements = target.Descendants().ToList();
        foreach (var node in xElements)
        {
            if (!serialization.TryGetScrubOrIgnoreByName(node.Name.LocalName, out var scrubOrIgnore))
            {
                continue;
            }

            if (scrubOrIgnore == ScrubOrIgnore.Ignore)
            {
                node.Remove();
            }
            else
            {
                node.Value = "Scrubbed";
            }
        }

        return Verify(target);
    }
}