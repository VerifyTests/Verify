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

    public override void Write(
        VerifyJsonWriter writer,
        StringBuilder value,
        JsonSerializer serializer)
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