using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(T target, VerifySettings settings)
        {
            if (target == null)
            {
                await VerifyString("null", settings);
                return;
            }

            if (VerifierSettings.TryGetToString(target, out var toString))
            {
                await VerifyString(toString!(target, settings), settings);
                return;
            }

            if (VerifierSettings.TryGetTypedConverter(target, settings, out var converter))
            {
                var result = await converter.Conversion(target!, settings);
                await VerifyBinary(result.Streams, settings.ExtensionOrTxt(), settings, result.Info, result.Cleanup);
                return;
            }

            if (target is Stream stream)
            {
                await VerifyStream(settings, stream);
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
                await VerifyBinary(streams, settings.ExtensionOrTxt(), settings, null, null);
                return;
            }

            await SerializeAndVerify(target, settings, VerifierSettings.GetJsonAppenders(settings));
        }

        Task SerializeAndVerify(object target, VerifySettings settings, List<ToAppend> appends)
        {
            var json = JsonFormatter.AsJson(
                target,
                settings.serialization.currentSettings,
                settings.IsNewLineEscapingDisabled,
                appends);
            return VerifyStringBuilder(json, settings);
        }
    }
}