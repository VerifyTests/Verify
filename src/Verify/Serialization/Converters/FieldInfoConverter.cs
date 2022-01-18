using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class FieldInfoConverter :
    WriteOnlyJsonConverter<FieldInfo>
{
    public override void WriteJson(
        VerifyJsonWriter writer,
        FieldInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.SimpleName());
    }
}