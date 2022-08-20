class ArgonJObjectConverter :
    WriteOnlyJsonConverter<JObject>
{
    public override void Write(VerifyJsonWriter writer, JObject value)
    {
        var dictionary = value.ToObject<DictionaryWrapper<string, object>>()!;
        writer.Serialize(dictionary);
    }
}