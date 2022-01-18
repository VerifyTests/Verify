using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class PropertyInfoConverter :
    WriteOnlyJsonConverter<PropertyInfo>
{
    public override void WriteJson(
        VerifyJsonTextWriter writer,
        PropertyInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.SimpleName());
    }
}