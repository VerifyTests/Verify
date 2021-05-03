using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifyTests;

class JObjectConverter :
    WriteOnlyJsonConverter<JObject>
{
    public override void WriteJson(
        JsonWriter writer,
        JObject value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        var dictionary = value.ToObject<Dictionary<string, object>>()!;
        serializer.Serialize(writer, dictionary);
    }
}