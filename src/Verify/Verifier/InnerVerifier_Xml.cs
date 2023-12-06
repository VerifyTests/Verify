namespace VerifyTests;

partial class InnerVerifier
{
    public async Task<VerifyResult> VerifyXml(Task<string> target) =>
        await VerifyXml(await target);

    public async Task<VerifyResult> VerifyXml(ValueTask<string> target) =>
        await VerifyXml(await target);

    public Task<VerifyResult> VerifyXml(string? target)
    {
        if (target is null)
        {
            return VerifyInner(target, null, emptyTargets, true);
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
            return await VerifyInner(target, null, emptyTargets, true);
        }

#if NET5_0_OR_GREATER
        var document = await XDocument.LoadAsync(target, LoadOptions.None, default);
#else
        var document = XDocument.Load(target, LoadOptions.None);
#endif
        return await VerifyXml(document);
    }

    async Task<VerifyResult> VerifyXml(XmlNode? target)
    {
        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, true);
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
            return VerifyInner(target, null, emptyTargets, true);
        }

        var serialization = settings.serialization;
        var nodes = target
            .Descendants()
            .ToList();
        foreach (var node in nodes)
        {
            if (serialization.TryGetScrubOrIgnoreByName(node.Name.LocalName, out var scrubOrIgnore))
            {
                if (scrubOrIgnore == ScrubOrIgnore.Ignore)
                {
                    node.Remove();
                }
                else
                {
                    node.Value = "Scrubbed";
                }

                continue;
            }

            foreach (var attribute in node
                         .Attributes()
                         .ToList())
            {
                if (serialization.TryGetScrubOrIgnoreByName(attribute.Name.LocalName, out scrubOrIgnore))
                {
                    if (scrubOrIgnore == ScrubOrIgnore.Ignore)
                    {
                        attribute.Remove();
                    }
                    else
                    {
                        attribute.Value = "Scrubbed";
                    }

                    continue;
                }

                attribute.Value = ConvertValue(serialization, attribute.Value);
            }

            if (node.IsEmpty || node.HasElements)
            {
                continue;
            }

            node.Value = ConvertValue(serialization, node.Value);
        }

        return VerifyString(target.ToString(), "xml");
    }

    string ConvertValue(SerializationSettings serialization, string value)
    {
        if (serialization.TryConvertString(counter, value, out var result))
        {
            return result;
        }

        return ApplyScrubbers.ApplyForPropertyValue(value, settings, counter);
    }
}