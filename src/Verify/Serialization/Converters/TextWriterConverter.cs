using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VerifyTests;

class TextWriterConverter :
    WriteOnlyJsonConverter<TextWriter>
{
    SharedScrubber sharedScrubber;

    public TextWriterConverter(SharedScrubber sharedScrubber)
    {
        this.sharedScrubber = sharedScrubber;
    }

    public override void WriteJson(
        JsonWriter writer,
        TextWriter value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        var stringValue = value.ToString();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (stringValue == null)
        {
            return;
        }

        if (sharedScrubber.TryConvertString(stringValue, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(stringValue);
    }
}