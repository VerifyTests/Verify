using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        static IEnumerable<Target> emptyTargets = Enumerable.Empty<Target>();

        public Task VerifyJson(string? target)
        {
            if (target == null)
            {
                AssertExtensionIsNull();
                return VerifyInner("null", null, emptyTargets);
            }

            return VerifyJson(JObject.Parse(target));
        }

        public async Task VerifyJson(Stream? target)
        {
            if (target == null)
            {
                AssertExtensionIsNull();
                await VerifyInner("null", null, emptyTargets);
                return;
            }

            using StreamReader reader = new (target);
            using JsonTextReader textReader = new(reader);
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
            if (target == null)
            {
                AssertExtensionIsNull();
                await VerifyInner("null", null, emptyTargets);
                return;
            }

            if (VerifierSettings.TryGetToString(target, out var toString))
            {
                var stringResult = toString(target, settings.Context);
                if (stringResult.Extension != null)
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
                var result = await converter.Conversion(target!, settings.Context);
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
                var streams = enumerable.Cast<Stream>()
                    .Select(x =>
                    {
                        if (x == null)
                        {
                            return new Target(settings.ExtensionOrTxt(), "null");
                        }

                        return new Target(settings.ExtensionOrBin(), x);
                    });
                await VerifyInner(null, null, streams);
                return;
            }

            AssertExtensionIsNull();
            await VerifyInner(target, null, emptyTargets);
        }

        void AssertExtensionIsNull()
        {
            if (settings.extension == null)
            {
                return;
            }

            throw new($@"{nameof(VerifySettings)}.{nameof(VerifySettings.UseExtension)}() should only be used for text, for streams, or for converter discovery.
When serializing an instance the default is txt.
To use json as an extension when serializing use {nameof(VerifierSettings)}.{nameof(VerifierSettings.UseStrictJson)}().");
        }
    }
}