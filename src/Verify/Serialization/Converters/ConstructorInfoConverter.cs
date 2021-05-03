using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using VerifyTests;

class ConstructorInfoConverter :
    WriteOnlyJsonConverter<ConstructorInfo>
{
    public override void WriteJson(
        JsonWriter writer, ConstructorInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(TypeNameConverter.GetName(value));
    }
}