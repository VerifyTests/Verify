using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VerifyTests;

class TypeJsonConverter :
    WriteOnlyJsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value == null)
        {
            return;
        }

        var type = (Type) value;
        writer.WriteValue(TypeNameConverter.GetName(type));
    }

    public override bool CanConvert(Type type)
    {
        return typeof(Type).IsAssignableFrom(type);
    }
}