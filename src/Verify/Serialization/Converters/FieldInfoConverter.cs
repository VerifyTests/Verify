using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class FieldInfoConverter :
    WriteOnlyJsonConverter<FieldInfo>
{
    public override void WriteJson(
        JsonWriter writer,
        FieldInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.SimpleName());
    }
}