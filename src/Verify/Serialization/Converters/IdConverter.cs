using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VerifyTests;

class IdConverter :
    WriteOnlyJsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        var id = CounterContext.Current.NextId(value);
        writer.WriteValue($"Id_{id}");
    }
}