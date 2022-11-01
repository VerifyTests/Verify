class JObjectConverter :
    WriteOnlyJsonConverter<JObject>
{
    public override void Write(VerifyJsonWriter writer, JObject value)
    {
        var dictionary = value
            .ToObject<OrderedDictionary>(writer.Serializer)!;
        writer.Serialize(dictionary);
    }
}