using System;
using Newtonsoft.Json;
using Verify;

class StringScrubbingConverter :
    WriteOnlyJsonConverter
{
    SharedScrubber sharedScrubber;

    public StringScrubbingConverter(SharedScrubber sharedScrubber)
    {
        this.sharedScrubber = sharedScrubber;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        var valueAsString = (string) value;
        if (sharedScrubber.TryConvertString(valueAsString, out var result))
        {
            writer.WriteRawValue(result);
            return;
        }

        writer.WriteValue(valueAsString);
    }

    public override bool CanRead => false;

    public override bool CanConvert(Type type)
    {
        return type == typeof(string);
    }
}