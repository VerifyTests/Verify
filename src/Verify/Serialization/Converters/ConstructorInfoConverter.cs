using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class ConstructorInfoConverter :
    WriteOnlyJsonConverter<ConstructorInfo>
{
    public override void WriteJson(
        VerifyJsonTextWriter writer,
        ConstructorInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.SimpleName());
    }
}