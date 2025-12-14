namespace VerifyTests;

partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyXml(Task<string> target) =>
        await VerifyXml(await target);

    public async Task<VerifyResult> VerifyXml(ValueTask<string> target) =>
        await VerifyXml(await target);

    public Task<VerifyResult> VerifyXml(
        [StringSyntax(StringSyntaxAttribute.Xml)]
        string? target)
    {
        if (target is null)
        {
            return VerifyInner(target, null, emptyTargets, false);
        }

        return VerifyXml(XDocument.Parse(target));
    }

    public async Task<VerifyResult> VerifyXml(Task<Stream> target) =>
        await VerifyXml(await target);

    public async Task<VerifyResult> VerifyXml(ValueTask<Stream> target) =>
        await VerifyXml(await target);

    // ReSharper disable once ReplaceAsyncWithTaskReturn
    public async Task<VerifyResult> VerifyXml(Stream? target)
    {
        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, false);
        }

        var document = await XDocument.LoadAsync(target, LoadOptions.None, default);
        return await VerifyXml(document);
    }

    async Task<VerifyResult> VerifyXml(XmlNode? target)
    {
        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, false);
        }

        using var reader = new XmlNodeReader(target);
        // ReSharper disable once MethodHasAsyncOverload
        reader.MoveToContent();
        return await VerifyXml(XDocument.Load(reader));
    }

    Task<VerifyResult> VerifyXml(XContainer? target)
    {
        if (target is null)
        {
            return VerifyInner(target, null, emptyTargets, false);
        }

        var xml = XmlScrubber.Scrub(target, settings);
        return VerifyString(xml, "xml");
    }
}