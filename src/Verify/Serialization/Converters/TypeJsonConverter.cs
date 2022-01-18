using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class TypeJsonConverter :
    WriteOnlyJsonConverter<Type>
{
    public override void WriteJson(
        VerifyJsonWriter writer,
        Type value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.SimpleName());
    }
}