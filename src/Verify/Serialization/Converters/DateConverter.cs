#if NET6_0_OR_GREATER

using Newtonsoft.Json;
using VerifyTests;

class DateConverter :
    WriteOnlyJsonConverter<DateOnly>
{
    SharedScrubber scrubber;

    public DateConverter(SharedScrubber scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void WriteJson(
        VerifyJsonTextWriter writer,
        DateOnly value,
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
#endif