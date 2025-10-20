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
            return VerifyInner(target, null, emptyTargets, true, false);
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
            return await VerifyInner(target, null, emptyTargets, true, false);
        }

        var document = await XDocument.LoadAsync(target, LoadOptions.None, default);
        return await VerifyXml(document);
    }

    async Task<VerifyResult> VerifyXml(XmlNode? target)
    {
        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, true, false);
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
            return VerifyInner(target, null, emptyTargets, true, false);
        }

        var serialization = settings.serialization;
        var elements = target
            .Descendants()
            .ToList();

        foreach (var element in elements)
        {
            if (serialization.TryGetScrubOrIgnoreByName(element.Name.LocalName, out var scrubOrIgnore))
            {
                if (scrubOrIgnore == ScrubOrIgnore.Ignore)
                {
                    element.Remove();
                }
                else
                {
                    element.Value = "Scrubbed";
                }

                continue;
            }

            ScrubAttributes(element, serialization);
        }

        foreach (var node in target.DescendantNodes())
        {
            switch (node)
            {
                case XText text:
                    text.Value = ConvertValue(text.Value);
                    continue;
                case XComment comment:
                    comment.Value = ConvertValue(comment.Value);
                    continue;
            }
        }

        return VerifyString(target.ToString(), "xml");
    }

    string ConvertValue(string value)
    {
        var span = value.AsSpan();
        if (counter.TryConvert(span, out var result))
        {
            return result;
        }

        return ApplyScrubbers.ApplyForPropertyValue(span, settings, counter).ToString();
    }

    void ScrubAttributes(XElement node, SerializationSettings serialization)
    {
        foreach (var attribute in node
                     .Attributes()
                     .ToList())
        {
            if (serialization.TryGetScrubOrIgnoreByName(attribute.Name.LocalName, out var scrubOrIgnore))
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

            var span = attribute.Value.AsSpan();
            if (counter.TryConvert(span, out var result))
            {
                attribute.Value = result;
            }
            else
            {
                attribute.Value = ApplyScrubbers.ApplyForPropertyValue(span, settings, counter).ToString();
            }
        }
    }
}