class DateTimeConverter :
    WriteOnlyJsonConverter<DateTime>
{
    SerializationSettings scrubber;

    public DateTimeConverter(SerializationSettings scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void Write(VerifyJsonWriter writer, DateTime value)
    {
        if (scrubber.TryConvert(writer.Counter, value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}