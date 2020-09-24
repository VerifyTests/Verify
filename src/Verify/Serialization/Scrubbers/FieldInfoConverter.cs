using System;
using System.Reflection;
using Newtonsoft.Json;
using VerifyTests;

class FieldInfoConverter :
    WriteOnlyJsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        var field = (FieldInfo) value;
        writer.WriteValue(TypeNameConverter.GetName(field));
    }

    public override bool CanConvert(Type type)
    {
        return typeof(FieldInfo).IsAssignableFrom(type);
    }
}