using System;
using System.Reflection;
using Newtonsoft.Json;
using VerifyTests;

class ParameterInfoConverter :
    WriteOnlyJsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        var parameter = (ParameterInfo) value;
        writer.WriteValue(TypeNameConverter.GetName(parameter));
    }

    public override bool CanConvert(Type type)
    {
        return typeof(ParameterInfo).IsAssignableFrom(type);
    }
}