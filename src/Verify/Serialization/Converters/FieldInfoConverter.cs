using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class FieldInfoConverter :
    WriteOnlyJsonConverter<FieldInfo>
{
    public override void Write(
        VerifyJsonWriter writer,
        FieldInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.SimpleName());
    }
}