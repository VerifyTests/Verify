class JObjectConverter :
    WriteOnlyJsonConverter<JObject>
{
    public override void Write(VerifyJsonWriter writer, JObject value)
    {
        if (VerifierSettings.orderJsonObjects)
        {
            var dictionary = new Dictionary<string, object?>();
            foreach (var item in value)
            {
                dictionary.Add(item.Key, item.Value);
            }

            writer.Serialize(dictionary);
        }
        else
        {
            var dictionary = new OrderedDictionary();
            foreach (var item in value)
            {
                dictionary.Add(item.Key, item.Value);
            }

            writer.Serialize(dictionary);
        }
    }
}