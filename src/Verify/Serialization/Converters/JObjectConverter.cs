using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifyTests;

class JObjectConverter :
    WriteOnlyJsonConverter<JObject>
{
    public override void Write(
        VerifyJsonWriter writer,
        JObject value,
        JsonSerializer serializer)
    {
        var dictionary = value.ToObject<Dictionary<string, object>>()!;
        serializer.Serialize(writer, dictionary);
    }
}