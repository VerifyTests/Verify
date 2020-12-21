using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VerifyTests;

class DirectoryInfoConverter :
    WriteOnlyJsonConverter<DirectoryInfo>
{
    public override void WriteJson(JsonWriter writer, DirectoryInfo? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value is null)
        {
            return;
        }

        writer.WriteValue(value.ToString().Replace('\\','/'));
    }
}