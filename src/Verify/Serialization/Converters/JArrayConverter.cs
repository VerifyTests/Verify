class JArrayConverter :
    WriteOnlyJsonConverter<JArray>
{
    public override void Write(VerifyJsonWriter writer, JArray value)
    {
        var list = value.ToObject<List<object>>()!;
        writer.Serialize(list);
    }
}