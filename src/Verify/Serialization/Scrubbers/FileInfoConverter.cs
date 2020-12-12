using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VerifyTests;

class FileInfoConverter :
    WriteOnlyJsonConverter<FileInfo>
{
    public override void WriteJson(JsonWriter writer, FileInfo? value, JsonSerializer serializer, IReadOnlyDictionary<string, object> context)
    {
        if (value is null)
        {
            return;
        }

        writer.WriteRawValue(value.ToString().Replace('\\','/'));
    }
}