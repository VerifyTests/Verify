partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyXml(Task<string> task) =>
        await VerifyXml(await task);

    public Task<VerifyResult> VerifyXml(string? target)
    {
        if (target is null)
        {
            return VerifyInner(target, null, emptyTargets, true);
        }

        return VerifyXml(XDocument.Parse(target));
    }

    public async Task<VerifyResult> VerifyXml(Task<Stream> task) =>
        await VerifyXml(await task);

    public async Task<VerifyResult> VerifyXml(Stream? target)
    {
        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, true);
        }

#if NET5_0_OR_GREATER
        var document = await XDocument.LoadAsync(target, LoadOptions.None, default);
#else
        var document = XDocument.Load(target, LoadOptions.None);
#endif
        return await VerifyXml(document);
    }

    public async Task<VerifyResult> VerifyXml(Task<XmlDocument> task) =>
        await VerifyXml(await task);

    public async Task<VerifyResult> VerifyXml(XmlDocument? target)
    {
        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, true);
        }

        using var nodeReader = new XmlNodeReader(target);
        // ReSharper disable once MethodHasAsyncOverload
        nodeReader.MoveToContent();
        return await VerifyXml(XDocument.Load(nodeReader));
    }

    public async Task<VerifyResult> VerifyXml(Task<XDocument> task) =>
        await VerifyXml(await task);

    public async Task<VerifyResult> VerifyXml(XDocument? target)
    {
        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, true);
        }

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

        var isText = FileExtensions.IsText("xml");
        return await VerifyString(target.ToString(), "xml");
    }
}