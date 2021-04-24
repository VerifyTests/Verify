using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using VerifyTests;

class ParameterInfoConverter :
    WriteOnlyJsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
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