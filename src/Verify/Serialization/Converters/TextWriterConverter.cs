class TextWriterConverter :
    WriteOnlyJsonConverter<TextWriter>
{
    public override void Write(VerifyJsonWriter writer, TextWriter value)
    {
        var stringValue = value.ToString();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (stringValue is null)
        {
            return;
        }

        writer.WriteValue(stringValue);
    }
}