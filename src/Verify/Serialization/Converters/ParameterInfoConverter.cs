using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class ParameterInfoConverter :
    WriteOnlyJsonConverter<ParameterInfo>
{
    public override void WriteJson(
        JsonWriter writer,
        ParameterInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.SimpleName());
    }
}