class NameValueCollectionConverter :
    WriteOnlyJsonConverter<NameValueCollection>
{
    public override void Write(VerifyJsonWriter writer, NameValueCollection collection)
    {
        var dictionary = new Dictionary<string, string?>(collection.Count);
        foreach (string? key in collection)
        {
            var value = collection.Get(key);

            var notNullKey = key ?? "null";

            value ??= "null";

            dictionary[notNullKey] = value;
        }

        writer.Serialize(dictionary);
    }
}