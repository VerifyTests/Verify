using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using VerifyTests;

class GuidConverter :
    WriteOnlyJsonConverter<Guid>
{
    SharedScrubber scrubber;

    public GuidConverter(SharedScrubber scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void WriteJson(
        JsonWriter writer,
        Guid value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (scrubber.TryConvert(value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value.ToString("D", CultureInfo.InvariantCulture));
    }
}