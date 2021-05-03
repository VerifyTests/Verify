using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;
using VerifyTests;

class NameValueCollectionConverter :
    WriteOnlyJsonConverter<NameValueCollection>
{
    public override void WriteJson(
        JsonWriter writer,
        NameValueCollection collection,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        Dictionary<string,string?> dictionary = new();
        foreach (string? key in collection)
        {
            var value = collection.Get(key);

            string? notNullKey;
            if (key == null)
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

        serializer.Serialize(writer,dictionary);
    }
}