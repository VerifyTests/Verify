partial class InnerVerifier
{
    static IEnumerable<Target> emptyTargets = Enumerable.Empty<Target>();

    public Task VerifyJson(string? target)
    {
        if (target is null)
        {
            AssertExtensionIsNull();
            return VerifyInner("null", null, emptyTargets);
        }

        return VerifyJson(JToken.Parse(target));
    }

    public async Task VerifyJson(Stream? target)
    {
        if (target is null)
        {
            AssertExtensionIsNull();
            await VerifyInner("null", null, emptyTargets);
            return;
        }

        using var reader = new StreamReader(target);
        using var textReader = new JsonTextReader(reader);
        var json = await JToken.LoadAsync(textReader);
        await VerifyJson(json);
    }

    public Task VerifyJson(JToken? target)
    {
        AssertExtensionIsNull();
        return VerifyInner(target, null, emptyTargets);
    }

    public async Task Verify<T>(T target)
    {
        if (target is null)
        {
            AssertExtensionIsNull();
            await VerifyInner("null", null, emptyTargets);
            return;
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
                await VerifyInner("emptyString", null, emptyTargets);
                return;
            }

            await VerifyInner(value, null, emptyTargets);
            return;
        }

        if (VerifierSettings.TryGetTypedConverter(target, settings, out var converter))
        {
           // AssertExtensionIsNull();
            var result = await converter.Conversion(target, settings.Context);
            await VerifyInner(result.Info, result.Cleanup, result.Targets);
            return;
        }

        if (target is Stream stream)
        {
            await VerifyStream(stream);
            return;
        }

        if (typeof(T).ImplementsStreamEnumerable())
        {
            var enumerable = (IEnumerable) target;
            var streams = enumerable.Cast<Stream>().Select(ToTarget);
            await VerifyInner(null, null, streams);
            return;
        }

        AssertExtensionIsNull();
        await VerifyInner(target, null, emptyTargets);
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
