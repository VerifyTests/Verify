class GuidConverter :
    WriteOnlyJsonConverter<Guid>
{
    SerializationSettings scrubber;

    public GuidConverter(SerializationSettings scrubber)
    {
        this.scrubber = scrubber;
    }

    public override void Write(VerifyJsonWriter writer, Guid value)
    {
        if (scrubber.TryConvert(writer.Counter, value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value.ToString("D", CultureInfo.InvariantCulture));
    }
}