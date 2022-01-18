using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class ParameterInfoConverter :
    WriteOnlyJsonConverter<ParameterInfo>
{
    public override void Write(
        VerifyJsonWriter writer,
        ParameterInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.SimpleName());
    }
}