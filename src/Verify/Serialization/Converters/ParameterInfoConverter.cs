using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using VerifyTests;

class ParameterInfoConverter :
    WriteOnlyJsonConverter<ParameterInfo>
{
    public override void WriteJson(
        JsonWriter writer,
        ParameterInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(TypeNameConverter.GetName(value));
    }
}