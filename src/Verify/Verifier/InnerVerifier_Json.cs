using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public async Task Verify<T>(T target, VerifySettings? settings = null)
        {
            Guard.AgainstNull(target, nameof(target));
            settings = settings.OrDefault();
            switch (target)
            {
                case string converted:
                    await VerifyString(converted, settings);
                    return;
                case DateTime converted:
                    await VerifyString(converted.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFz"), settings);
                    return;
                case DateTimeOffset converted:
                    await VerifyString(converted.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFz"), settings);
                    return;
                case int converted:
                    await VerifyString(converted.ToString(), settings);
                    return;
                case uint converted:
                    await VerifyString(converted.ToString(), settings);
                    return;
                case long converted:
                    await VerifyString(converted.ToString(), settings);
                    break;
                case ulong converted:
                    await VerifyString(converted.ToString(), settings);
                    break;
                case short converted:
                    await VerifyString(converted.ToString(), settings);
                    return;
                case ushort converted:
                    await VerifyString(converted.ToString(), settings);
                    return;
                case float converted:
                    await VerifyString(converted.ToString(), settings);
                    return;
                case Guid converted:
                    await VerifyString(converted.ToString(), settings);
                    return;
                case decimal converted:
                    await VerifyString(converted.ToString(), settings);
                    return;
                case bool converted:
                    await VerifyString(converted.ToString(), settings);
                    return;
                case XmlNode converted:
                    var document = XDocument.Parse(converted.OuterXml);
                    settings.UseExtension("xml");
                    await VerifyString(document.ToString(), settings);
                    return;
                case XDocument converted:
                    settings.UseExtension("xml");
                    await VerifyString(converted.ToString(), settings);
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
                var enumerable = (IEnumerable) target!;
                var streams = enumerable.Cast<Stream>().Select(x => new ConversionStream(settings.ExtensionOrBin(), x));
                await VerifyBinary(streams, settings.ExtensionOrTxt(), settings, null, null);
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