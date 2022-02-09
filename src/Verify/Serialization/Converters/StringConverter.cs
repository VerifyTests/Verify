class StringConverter :
    WriteOnlyJsonConverter<string>
{
    SerializationSettings settings;

    public StringConverter(SerializationSettings settings)
    {
        this.settings = settings;
    }

    public override void Write(VerifyJsonWriter writer, string value)
    {
        if (settings.TryConvertString(writer.Counter, value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}