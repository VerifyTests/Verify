using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VerifyTests;

class DateTimeOffsetConverter :
    WriteOnlyJsonConverter<DateTimeOffset>
{
    SharedScrubber scrubber;

    public DateTimeOffsetConverter(SharedScrubber scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void WriteJson(
        JsonWriter writer,
        DateTimeOffset value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (scrubber.TryConvert(value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}