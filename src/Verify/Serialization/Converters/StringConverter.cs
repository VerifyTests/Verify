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

    public override void Write(
        VerifyJsonWriter writer,
        string value,
        JsonSerializer serializer)
    {
        if (sharedScrubber.TryConvertString(value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}