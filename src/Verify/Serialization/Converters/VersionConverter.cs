using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using VerifyTests;

class VersionConverter :
    WriteOnlyJsonConverter<Version>
{
    public override void WriteJson(
        JsonWriter writer,
        Version value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.ToString());
    }
}