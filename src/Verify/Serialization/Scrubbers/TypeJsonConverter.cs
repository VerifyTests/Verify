using System;
using Newtonsoft.Json;
using VerifyTests;

class TypeJsonConverter :
    WriteOnlyJsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
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