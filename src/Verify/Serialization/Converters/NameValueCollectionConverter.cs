class NameValueCollectionConverter :
    WriteOnlyJsonConverter<NameValueCollection>
{
    public override void Write(VerifyJsonWriter writer, NameValueCollection collection)
    {
        var dictionary = new Dictionary<string,string?>();
        foreach (string? key in collection)
        {
            var value = collection.Get(key);

            string? notNullKey;
            if (key is null)
            {
                notNullKey = "null";
            }
            else
            {
                notNullKey = key;
            }

            value ??= "null";

            dictionary[notNullKey] = value;
        }

        writer.Serialize(dictionary);
    }
}