using System;
using System.Reflection;
using Newtonsoft.Json;
using VerifyTests;

class MethodInfoConverter :
    WriteOnlyJsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        var method = (MethodInfo) value;
        writer.WriteValue(TypeNameConverter.GetName(method));
    }

    public override bool CanConvert(Type type)
    {
        return typeof(MethodInfo).IsAssignableFrom(type);
    }
}