class JObjectConverter :
    WriteOnlyJsonConverter<JObject>
{
    public override void Write(VerifyJsonWriter writer, JObject value)
    {
        if (VerifierSettings.sortJsonObjects)
        {
            var dictionary = value
                .ToObject<Dictionary<string,object>>(writer.Serializer)!;
            writer.Serialize(dictionary);
        }
        else
        {
            var dictionary = value
                .ToObject<OrderedDictionary>(writer.Serializer)!;
            writer.Serialize(dictionary);
        }
    }
}