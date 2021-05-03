using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using VerifyTests;

class GuidConverter :
    WriteOnlyJsonConverter
{
    SharedScrubber scrubber;

    public GuidConverter(SharedScrubber scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void WriteJson(
        JsonWriter writer,
        object? value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (value == null)
        {
            return;
        }

        var guid = (Guid) value;
        if (scrubber.TryConvert(guid, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(guid.ToString("D", CultureInfo.InvariantCulture));
    }

    public override bool CanConvert(Type type)
    {
        return type == typeof(Guid) ||
               type == typeof(Guid?);
    }
}