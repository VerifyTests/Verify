using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(T target)
        {
            if (target == null)
            {
                await VerifyString("null");
                return;
            }

            if (VerifierSettings.TryGetToString(target, out var toString))
            {
                var asStringResult = toString!(target, settings.Context);
                if (asStringResult.Extension != null)
                {
                    settings.UseExtension(asStringResult.Extension);
                }

                await VerifyString(asStringResult.Value);
                return;
            }

            if (VerifierSettings.TryGetTypedConverter(target, settings, out var converter))
            {
                var result = await converter.Conversion(target!, settings.Context);
                await VerifyBinary(result.Streams, result.Info, result.Cleanup);
                return;
            }

            if (target is Stream stream)
            {
                await VerifyStream(stream, settings.extension);
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
                            return new ConversionStream(settings.ExtensionOrTxt(), "null");
                        }

                        return new ConversionStream(settings.ExtensionOrBin(), x);
                    });
                await VerifyBinary(streams, null, null);
                return;
            }

            var appenders = VerifierSettings.GetJsonAppenders(settings);

            await SerializeAndVerify(target, appenders);
        }

        Task SerializeAndVerify(object target, List<ToAppend> appends)
        {
            var json = JsonFormatter.AsJson(
                target,
                settings.serialization.currentSettings,
                appends,
                settings);

            var defaultValue = "txt";
            if (VerifierSettings.StrictJson)
            {
                defaultValue = "json";
            }

            var extension = settings.ExtensionOrTxt(defaultValue);
            return VerifyStringBuilder(json, extension);
        }
    }
}