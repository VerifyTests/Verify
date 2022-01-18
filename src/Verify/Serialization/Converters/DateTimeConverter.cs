using Newtonsoft.Json;
using VerifyTests;

class DateTimeConverter :
    WriteOnlyJsonConverter<DateTime>
{
    SharedScrubber scrubber;

    public DateTimeConverter(SharedScrubber scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void WriteJson(
        VerifyJsonWriter writer,
        DateTime value,
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