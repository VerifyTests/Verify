#if NET6_0_OR_GREATER

class DateConverter :
    WriteOnlyJsonConverter<DateOnly>
{
    SerializationSettings scrubber;

    public DateConverter(SerializationSettings scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void Write(VerifyJsonWriter writer, DateOnly value)
    {
        if (scrubber.TryConvert(writer.Counter, value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}
#endif