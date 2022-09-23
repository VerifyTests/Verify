﻿partial class InnerVerifier
{
    static IEnumerable<Target> emptyTargets = Enumerable.Empty<Target>();

    public Task<VerifyResult> VerifyJson(string target) =>
        VerifyJson(JToken.Parse(target));

    public async Task<VerifyResult> VerifyJson(Stream target)
    {
        using var reader = new StreamReader(target);
        using var textReader = new JsonTextReader(reader);
        var json = await JToken.LoadAsync(textReader);
        return await VerifyJson(json);
    }

    public Task<VerifyResult> VerifyJson(JToken target)
    {
        AssertExtensionIsNull();
        return VerifyInner(target, null, emptyTargets);
    }

    public async Task<VerifyResult> Verify(object target)
    {
        if (target is byte[] bytes)
        {
            return await VerifyStream(new MemoryStream(bytes));
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
            return await VerifyStream(stream);
        }

        if (target.GetType().ImplementsStreamEnumerable())
        {
            var enumerable = (IEnumerable) target;
            var targets = enumerable.Cast<Stream>()
                .Select<Stream, Target>(_ => new(settings.ExtensionOrBin(), _));
            return await VerifyInner(null, null, targets);
        }

        AssertExtensionIsNull();
        return await VerifyInner(target, null, emptyTargets);
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