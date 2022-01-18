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

    public override void Write(
        VerifyJsonWriter writer,
        DateTimeOffset value,
        JsonSerializer serializer)
    {
        if (scrubber.TryConvert(value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}