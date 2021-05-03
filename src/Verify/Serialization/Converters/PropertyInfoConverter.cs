using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
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
        writer.WriteValue(TypeNameConverter.GetName(value));
    }
}