using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifyTests;

class JArrayConverter :
    WriteOnlyJsonConverter<JArray>
{
    public override void WriteJson(
        JsonWriter writer,
        JArray value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        var list = value.ToObject<List<object>>()!;
        serializer.Serialize(writer, list);
    }
}