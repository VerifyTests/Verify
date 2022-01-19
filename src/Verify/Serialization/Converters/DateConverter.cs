#if NET6_0_OR_GREATER

using Newtonsoft.Json;
using VerifyTests;

class DateConverter :
    WriteOnlyJsonConverter<DateOnly>
{
    SerializationSettings scrubber;

    public DateConverter(SerializationSettings scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void Write(
        VerifyJsonWriter writer,
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