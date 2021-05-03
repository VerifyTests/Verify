using System.Collections.Generic;
using Newtonsoft.Json;
using VerifyTests;

class StringConverter :
    WriteOnlyJsonConverter<string>
{
    SharedScrubber sharedScrubber;

    public StringConverter(SharedScrubber sharedScrubber)
    {
        this.sharedScrubber = sharedScrubber;
    }

    public override void WriteJson(
        JsonWriter writer,
        string value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        if (sharedScrubber.TryConvertString(value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}