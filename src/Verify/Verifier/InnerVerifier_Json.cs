using System.Collections;
using System.Collections.Generic;
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

        if (SharedVerifySettings.TryGetConverter<T>(out var typeConverter))
        {
            var converterSettings = new VerifySettings(settings);
            converterSettings.UseExtension(typeConverter.ToExtension);
            var converterFunc = typeConverter.Func(input!, converterSettings);
            await VerifyBinary(converterFunc, converterSettings);
            return;
        }

        if (input is Stream stream)
        {
            using (stream)
            {
                if (settings.HasExtension())
                {
                    if (SharedVerifySettings.TryGetConverter(settings.extension!, out var converter))
                    {
                        var converterSettings = new VerifySettings(settings);
                        converterSettings.UseExtension(converter.ToExtension);
                        var streams = converter.Func(stream, converterSettings);
                        await VerifyBinary(streams, converterSettings);
                        return;
                    }
                }

                await VerifyBinary(new List<Stream>{stream}, settings);
                return;
            }
        }

        if (typeof(T).ImplementsStreamEnumerable())
        {
            var enumerable = (IEnumerable) input!;
            await VerifyBinary(enumerable.Cast<Stream>(), settings);
            return;
        }

        var formatJson = JsonFormatter.AsJson(input, settings.serialization.currentSettings);
        await Verify(formatJson, settings);
        return;
    }
}