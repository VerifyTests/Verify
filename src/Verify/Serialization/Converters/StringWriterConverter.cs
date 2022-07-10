class StringWriterConverter :
    WriteOnlyJsonConverter<StringWriter>
{
    public override void Write(VerifyJsonWriter writer, StringWriter value) =>
        writer.WriteValue(value.ToString());
}