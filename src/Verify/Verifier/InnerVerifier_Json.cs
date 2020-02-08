﻿using System;
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

        if (SharedVerifySettings.TryGetConverter<T>(
            settings.extension,
            out var converter))
        {
            var converterSettings = GetConverterSettings<T>(settings, converter);
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
        await Verify(formatJson, settings);
    }

    static VerifySettings GetConverterSettings<T>(VerifySettings settings, TypeConverter converter)
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
                    var result = await converter.Func(stream, converterSettings);
                    await VerifyBinary(result.Streams, converterSettings, result.Info);
                    return;
                }
            }

            await VerifyBinary(new List<Stream> {stream}, settings, null);
        }
    }
}