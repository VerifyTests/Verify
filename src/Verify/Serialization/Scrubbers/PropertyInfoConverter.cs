using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using VerifyTests;

class PropertyInfoConverter :
    WriteOnlyJsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value == null)
        {
            return;
        }

        var property = (PropertyInfo) value;
        writer.WriteValue(TypeNameConverter.GetName(property));
    }

    public override bool CanConvert(Type type)
    {
        return typeof(PropertyInfo).IsAssignableFrom(type);
    }
}