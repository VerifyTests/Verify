﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Verify;

partial class InnerVerifier
{
    public Task Verify<T>(T input, VerifySettings? settings = null)
    {
        Guard.AgainstNull(input, nameof(input));
        settings = settings.OrDefault();

        if (SharedVerifySettings.TryGetConverter<T>(out var typeConverter))
        {
            var converterSettings = new VerifySettings(settings);
            converterSettings.UseExtension(typeConverter.ToExtension);
            var result = typeConverter.Func(input!, converterSettings);
            return VerifyBinary(result.Streams, converterSettings, result.Info);
        }

        if (input is Stream stream)
        {
            return VerifyStream(settings, stream);
        }

        if (typeof(T).ImplementsStreamEnumerable())
        {
            var enumerable = (IEnumerable) input!;
            return VerifyBinary(enumerable.Cast<Stream>(), settings, null);
        }

        var formatJson = JsonFormatter.AsJson(input, settings.serialization.currentSettings);
        return Verify(formatJson, settings);
    }

    async Task VerifyStream(VerifySettings settings, Stream stream)
    {
        using (stream)
        {
            if (settings.HasExtension())
            {
                if (SharedVerifySettings.TryGetConverter(settings.extension!, out var converter))
                {
                    var converterSettings = new VerifySettings(settings);
                    converterSettings.UseExtension(converter.ToExtension);
                    var result = converter.Func(stream, converterSettings);
                    await VerifyBinary(result.Streams, converterSettings, result.Info);
                    return;
                }
            }

            await VerifyBinary(new List<Stream> {stream}, settings, null);
        }
    }
}