namespace VerifyTests;

partial class InnerVerifier
{
    static IEnumerable<Target> emptyTargets = Enumerable.Empty<Target>();

    public async Task<VerifyResult> Verify(object? target)
    {
        if (target is string stringTarget)
        {
            return await VerifyString(stringTarget);
        }

        if (target is XContainer container)
        {
            return await VerifyXml(container);
        }

        if (target is XmlNode node)
        {
            return await VerifyXml(node);
        }

        if (target is FileStream fileStream)
        {
            return await VerifyStream(fileStream, null);
        }

        if (target is Stream stream)
        {
            return await VerifyStream(stream, null);
        }

        if (target is byte[] bytes)
        {
            return await VerifyStream(bytes, null);
        }

        if (target is null)
        {
            return await VerifyInner(target, null, emptyTargets, true);
        }

        if (target is IEnumerable<Stream>)
        {
            throw new("Use Verify(IEnumerable<T> targets, string extension)");
        }

        if (VerifierSettings.TryGetToString(target, out var toString))
        {
            var stringResult = toString(target, settings.Context);
            if (stringResult.Extension is null)
            {
                return await VerifyString(stringResult.Value);
            }

            return await VerifyString(stringResult.Value, stringResult.Extension);
        }

        if (VerifierSettings.TryGetTypedConverter(target, settings, out var converter))
        {
            var result = await converter.Conversion(target, settings.Context);
            return await VerifyInner(result.Info, result.Cleanup, result.Targets, true);
        }

        return await VerifyInner(target, null, emptyTargets, true);
    }
}