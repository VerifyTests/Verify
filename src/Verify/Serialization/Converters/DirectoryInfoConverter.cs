using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VerifyTests;

class DirectoryInfoConverter :
    WriteOnlyJsonConverter<DirectoryInfo>
{
    public override void WriteJson(
        JsonWriter writer,
        DirectoryInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.ToString().Replace('\\','/'));
    }
}