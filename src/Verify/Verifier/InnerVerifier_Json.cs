using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(T target, VerifySettings? settings = null)
        {
            Guard.AgainstNull(target, nameof(target));
            settings = settings.OrDefault();

            if (VerifierSettings.TryGetTypedConverter(target, out var converter))
            {
                var result = await converter.Conversion(target!, settings);
                await VerifyBinary(result.Streams,result.StreamExtension, settings, result.Info, result.Cleanup);
                return;
            }

            if (target is Stream stream)
            {
                await VerifyStream(settings, stream);
                return;
            }

            if (typeof(T).ImplementsStreamEnumerable())
            {
                var enumerable = (IEnumerable) target!;
                await VerifyBinary(enumerable.Cast<Stream>(),settings.ExtensionOrBin(), settings, null, null);
                return;
            }

            var formatJson = JsonFormatter.AsJson(
                target,
                settings.serialization.currentSettings,
                settings.IsNewLineEscapingDisabled);
            await Verify(formatJson, settings);
        }
    }
}