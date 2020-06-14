using System;
using Newtonsoft.Json;
using VerifyTests;

class DateTimeConverter :
    WriteOnlyJsonConverter
{
    SharedScrubber scrubber;

    public DateTimeConverter(SharedScrubber scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        var dateTime = (DateTime) value;
        if(scrubber.TryConvert(dateTime, out var result))
        {
            writer.WriteRawValue(result);
            return;
        }
        writer.WriteValue(dateTime);
    }

    public override bool CanConvert(Type type)
    {
        return type == typeof(DateTime) ||
               type == typeof(DateTime?);
    }
}