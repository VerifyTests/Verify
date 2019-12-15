using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Verify;

partial class Verifier
{
    public Task Verify<T>(T input, VerifySettings? settings = null)
    {
        Guard.AgainstNull(input, nameof(input));
        settings = settings.OrDefault();

        if (SharedVerifySettings.TryGetConverter<T>(out var typeConverter))
        {
            var converterSettings = new VerifySettings(settings);
            converterSettings.UseExtension(typeConverter.ToExtension);
            var converterFunc = typeConverter.Func(input!);
            return VerifyMultipleBinary(converterFunc, converterSettings);
        }

        if (input is Stream stream)
        {
            if (settings.HasExtension())
            {
                if (SharedVerifySettings.TryGetConverter(settings.extension!, out var converter))
                {
                    var converterSettings = new VerifySettings(settings);
                    converterSettings.UseExtension(converter.ToExtension);
                    var streams = converter.Func(stream);
                    return VerifyMultipleBinary(streams, converterSettings);
                }
            }
            return VerifyBinary(stream, settings);
        }

        if (typeof(T).IsStreamEnumerable())
        {
            var enumerable = (IEnumerable)input!;
            return VerifyMultipleBinary(enumerable.Cast<Stream>(), settings);
        }
        var formatJson = JsonFormatter.AsJson(input, settings.serialization.currentSettings);
        return Verify(formatJson, settings);
    }
}