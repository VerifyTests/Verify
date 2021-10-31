using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class ConstructorInfoConverter :
    WriteOnlyJsonConverter<ConstructorInfo>
{
    public override void WriteJson(
        JsonWriter writer, ConstructorInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.SimpleName());
    }
}