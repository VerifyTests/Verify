using System.Collections;
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
                await VerifyInner(null, null, Enumerable.Empty<ConversionStream>());
                return;
            }

            if (VerifierSettings.TryGetToString(target, out var toString))
            {
                var stringResult = toString(target, settings.Context);
                if (stringResult.Extension != null)
                {
                    settings.UseExtension(stringResult.Extension);
                }

                await VerifyInner(stringResult.Value, null, Enumerable.Empty<ConversionStream>());
                return;
            }

            if (VerifierSettings.TryGetTypedConverter(target, settings, out var converter))
            {
                var result = await converter.Conversion(target!, settings.Context);
                await VerifyInner(result.Info, result.Cleanup, result.Streams);
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
                await VerifyInner(null, null, streams);
                return;
            }

            await VerifyInner(target, null, Enumerable.Empty<ConversionStream>());
        }
    }
}