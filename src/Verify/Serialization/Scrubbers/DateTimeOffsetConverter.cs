using System;
using Newtonsoft.Json;
using VerifyTesting;

class DateTimeOffsetConverter :
    WriteOnlyJsonConverter
{
    SharedScrubber scrubber;

    public DateTimeOffsetConverter(SharedScrubber scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        var dateTime = (DateTimeOffset) value;
        if(scrubber.TryConvert(dateTime, out var result))
        {
            writer.WriteRawValue(result);
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