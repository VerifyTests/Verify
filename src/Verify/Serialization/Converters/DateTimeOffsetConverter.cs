class DateTimeOffsetConverter :
    WriteOnlyJsonConverter<DateTimeOffset>
{
    SerializationSettings scrubber;

    public DateTimeOffsetConverter(SerializationSettings scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void Write(VerifyJsonWriter writer, DateTimeOffset value)
    {
        if (scrubber.TryConvert(writer.Counter, value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}