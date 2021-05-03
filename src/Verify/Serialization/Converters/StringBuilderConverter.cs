using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using VerifyTests;

class StringBuilderConverter :
    WriteOnlyJsonConverter<StringBuilder>
{
    SharedScrubber sharedScrubber;

    public StringBuilderConverter(SharedScrubber sharedScrubber)
    {
        this.sharedScrubber = sharedScrubber;
    }

    public override void WriteJson(
        JsonWriter writer,
        StringBuilder value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        var stringValue = value.ToString();
        if (sharedScrubber.TryConvertString(stringValue, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(stringValue);
    }
}