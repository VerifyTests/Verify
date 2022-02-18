class TextWriterConverter :
    WriteOnlyJsonConverter<TextWriter>
{
    SerializationSettings settings;

    public TextWriterConverter(SerializationSettings settings)
    {
        this.settings = settings;
    }

    public override void Write(VerifyJsonWriter writer, TextWriter value)
    {
        var stringValue = value.ToString();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (stringValue is null)
        {
            return;
        }

        if (settings.TryConvertString(writer.Counter, stringValue, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(stringValue);
    }
}