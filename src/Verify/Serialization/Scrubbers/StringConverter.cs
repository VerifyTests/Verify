using System;
using Newtonsoft.Json;
using Verify;

class StringConverter :
    WriteOnlyJsonConverter
{
    SharedScrubber sharedScrubber;

    public StringConverter(SharedScrubber sharedScrubber)
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

        var replace = valueAsString.Replace("\r\n", "\n").Replace("\r", "\n");
        writer.WriteValue(replace);
    }

    public override bool CanRead => false;

    public override bool CanConvert(Type type)
    {
        return type == typeof(string);
    }
}