using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VerifyTests;

class StringConverter :
    WriteOnlyJsonConverter
{
    SharedScrubber sharedScrubber;

    public StringConverter(SharedScrubber sharedScrubber)
    {
        this.sharedScrubber = sharedScrubber;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
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