using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class ConstructorInfoConverter :
    WriteOnlyJsonConverter<ConstructorInfo>
{
    public override void Write(
        VerifyJsonWriter writer,
        ConstructorInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.SimpleName());
    }
}