using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using VerifyTests;


class ConstructorInfoConverter :
    WriteOnlyJsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value == null)
        {
            return;
        }

        var constructor = (ConstructorInfo) value;
        writer.WriteValue(TypeNameConverter.GetName(constructor));
    }

    public override bool CanConvert(Type type)
    {
        return typeof(ConstructorInfo).IsAssignableFrom(type);
    }
}