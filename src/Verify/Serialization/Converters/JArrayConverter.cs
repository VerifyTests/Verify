using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifyTests;

class JArrayConverter :
    WriteOnlyJsonConverter<JArray>
{
    public override void Write(
        VerifyJsonWriter writer,
        JArray value,
        JsonSerializer serializer)
    {
        var list = value.ToObject<List<object>>()!;
        serializer.Serialize(writer, list);
    }
}