using System;
using System.Collections.Generic;
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

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value == null)
        {
            return;
        }

        var guid = (Guid) value;
        if(scrubber.TryConvert(guid, out var result))
        {
            writer.WriteRawValue(result);
            return;
        }
        writer.WriteValue(guid);
    }

    public override bool CanConvert(Type type)
    {
        return type == typeof(Guid) ||
               type == typeof(Guid?);
    }
}