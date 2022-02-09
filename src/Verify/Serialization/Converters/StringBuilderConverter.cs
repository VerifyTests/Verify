class StringBuilderConverter :
    WriteOnlyJsonConverter<StringBuilder>
{
    SerializationSettings settings;

    public StringBuilderConverter(SerializationSettings settings)
    {
        this.settings = settings;
    }

    public override void Write(VerifyJsonWriter writer, StringBuilder value)
    {
        var stringValue = value.ToString();
        if (settings.TryConvertString(writer.Counter, stringValue, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(stringValue);
    }
}