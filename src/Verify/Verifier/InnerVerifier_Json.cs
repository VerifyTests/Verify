using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Verify;

partial class InnerVerifier
{
    public async Task Verify<T>(T input, VerifySettings? settings = null)
    {
        Guard.AgainstNull(input, nameof(input));
        settings = settings.OrDefault();

        if (SharedVerifySettings.TryGetConverter<T>(
            settings.extension,
            out var converter))
        {
            var converterSettings = GetConverterSettings(settings, converter);
            var result = await converter.Func(input!, converterSettings);
            await VerifyBinary(result.Streams, converterSettings, result.Info);
            return;
        }

        if (input is Stream stream)
        {
            await VerifyStream(settings, stream);
            return;
        }

        if (typeof(T).ImplementsStreamEnumerable())
        {
            var enumerable = (IEnumerable) input!;
            await VerifyBinary(enumerable.Cast<Stream>(), settings, null);
            return;
        }
        var formatJson = JsonFormatter.AsJson(input, settings.serialization.currentSettings);
        if (settings.newLineEscapingDisabled || SharedVerifySettings.newLineEscapingDisabled)
        {
            formatJson = formatJson.Replace("\\r\\n", "\n");
        }
        await Verify(formatJson, settings);
    }

    static VerifySettings GetConverterSettings(VerifySettings settings, TypeConverter converter)
    {
        if (converter.ToExtension != null)
        {
            var converterSettings = new VerifySettings(settings);
            converterSettings.UseExtension(converter.ToExtension);
            return converterSettings;
        }

        if (settings.HasExtension())
        {
            return settings;
        }

        throw new Exception("No extension defined.");
    }
}