using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VerifyTests;

class FileInfoConverter :
    WriteOnlyJsonConverter<FileInfo>
{
    public override void WriteJson(
        JsonWriter writer,
        FileInfo value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.ToString().Replace('\\','/'));
    }
}