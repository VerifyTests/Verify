class StringBuilderConverter :
    WriteOnlyJsonConverter<StringBuilder>
{
    public override void Write(VerifyJsonWriter writer, StringBuilder value)
    {
        writer.WriteValue(value.ToString());
    }
}