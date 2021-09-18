using Newtonsoft.Json;
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
        writer.WriteValue(TypeNameConverter.GetName(value));
    }
}