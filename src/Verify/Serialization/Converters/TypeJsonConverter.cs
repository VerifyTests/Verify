using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VerifyTests;

class TypeJsonConverter :
    WriteOnlyJsonConverter<Type>
{
    public override void WriteJson(
        JsonWriter writer,
        Type value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(TypeNameConverter.GetName(value));
    }
}