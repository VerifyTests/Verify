class StringDictionaryConverter :
    WriteOnlyJsonConverter<StringDictionary>
{
    public override void Write(VerifyJsonWriter writer, StringDictionary collection)
    {
        var dictionary = new Dictionary<string, string?>(collection.Count);
        foreach (string? key in collection.Keys)
        {
            var notNullKey = key!;
            var value = collection[notNullKey];
            dictionary[notNullKey] = value;
        }

        writer.Serialize(dictionary);
    }
}