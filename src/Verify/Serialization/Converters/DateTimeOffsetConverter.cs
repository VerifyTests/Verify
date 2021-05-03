using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VerifyTests;

class DateTimeOffsetConverter :
    WriteOnlyJsonConverter
{
    SharedScrubber scrubber;

    public DateTimeOffsetConverter(SharedScrubber scrubber)
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

        var dateTime = (DateTimeOffset) value;
        if(scrubber.TryConvert(dateTime, out var result))
        {
            writer.WriteValue(result);
            return;
        }
        writer.WriteValue(dateTime);
    }

    public override bool CanConvert(Type type)
    {
        return type == typeof(DateTimeOffset) ||
               type == typeof(DateTimeOffset?);
    }
}