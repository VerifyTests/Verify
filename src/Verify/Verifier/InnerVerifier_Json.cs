partial class InnerVerifier
{
    static IEnumerable<Target> emptyTargets = Enumerable.Empty<Target>();

    public Task<VerifyResult> VerifyJson(string? target)
    {
        if (target is null)
        {
            AssertExtensionIsNull();
            return VerifyInner("null", null, emptyTargets);
        }

        return VerifyJson(JToken.Parse(target));
    }

    public async Task<VerifyResult> VerifyJson(Stream? target)
    {
        if (target is null)
        {
            AssertExtensionIsNull();
            return await VerifyInner("null", null, emptyTargets);
        }

        using var reader = new StreamReader(target);
        using var textReader = new JsonTextReader(reader);
        var json = await JToken.LoadAsync(textReader);
        return await VerifyJson(json);
    }

    public Task<VerifyResult> VerifyJson(JToken? target)
    {
        AssertExtensionIsNull();
        return VerifyInner(target, null, emptyTargets);
    }

    public async Task<VerifyResult> Verify(object? target)
    {
        if (target is null)
        {
            AssertExtensionIsNull();
            return await VerifyInner("null", null, emptyTargets);
        }

        if (target is byte[] bytes)
        {
            throw new("Use Verify(byte[] bytes, string extension)");
        }

        if (VerifierSettings.TryGetToString(target, out var toString))
        {
            var stringResult = toString(target, settings.Context);
            if (stringResult.Extension is not null)
            {
                settings.UseExtension(stringResult.Extension);
            }

            var value = stringResult.Value;
            if (value == string.Empty)
            {
                return await VerifyInner("emptyString", null, emptyTargets);
            }

            return await VerifyInner(value, null, emptyTargets);
        }

        if (VerifierSettings.TryGetTypedConverter(target, settings, out var converter))
        {
            var result = await converter.Conversion(target, settings.Context);
            return await VerifyInner(result.Info, result.Cleanup, result.Targets);
        }

        if (target is Stream stream)
        {
            throw new("Use Verify(Stream stream, string extension)");
        }

        if (target.GetType().ImplementsStreamEnumerable())
        {
            throw new("Use Verify(IEnumerable<Stream> streams, string extension)");
        }

        AssertExtensionIsNull();
        return await VerifyInner(target, null, emptyTargets);
    }

    Target ToTarget(Stream? stream)
    {
        if (stream is null)
        {
            return new(settings.ExtensionOrTxt(), "null");
        }

        return new(settings.ExtensionOrBin(), stream);
    }

    void AssertExtensionIsNull()
    {
        if (settings.extension is null)
        {
            return;
        }

        throw new($@"{nameof(VerifySettings)}.{nameof(VerifySettings.UseExtension)}() should only be used for text, for streams, or for converter discovery.
When serializing an instance the default is txt.
To use json as an extension when serializing use {nameof(VerifierSettings)}.{nameof(VerifierSettings.UseStrictJson)}().");
    }
}