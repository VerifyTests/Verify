using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class MethodInfoConverter :
    WriteOnlyJsonConverter<MethodInfo>
{
    public override void WriteJson(
        JsonWriter writer,
        MethodInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.SimpleName());
    }
}