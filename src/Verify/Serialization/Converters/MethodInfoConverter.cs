using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class MethodInfoConverter :
    WriteOnlyJsonConverter<MethodInfo>
{
    public override void WriteJson(
        VerifyJsonTextWriter writer,
        MethodInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.SimpleName());
    }
}