class ToAppendConverter :
    WriteOnlyJsonConverter<ToAppend>
{
    public override void Write(VerifyJsonWriter writer, ToAppend value)
    {
        writer.WriteStartObject();
        writer.WriteMember(value, value.Data, value.Name);
        writer.WriteEndObject();
    }
}