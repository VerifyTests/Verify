class JObjectConverter :
    WriteOnlyJsonConverter<JObject>
{
    public override void Write(VerifyJsonWriter writer, JObject value)
    {
        var dictionary = value.ToObject<Dictionary<string, object>>()!;
        writer.Serialize(dictionary);
    }
}