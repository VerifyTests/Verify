using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class PropertyInfoConverter :
    WriteOnlyJsonConverter<PropertyInfo>
{
    public override void WriteJson(
        JsonWriter writer,
        PropertyInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.SimpleName());
    }
}