using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using VerifyTests;

class FieldInfoConverter :
    WriteOnlyJsonConverter<FieldInfo>
{
    public override void WriteJson(
        JsonWriter writer,
        FieldInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(TypeNameConverter.GetName(value));
    }
}