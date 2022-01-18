using Newtonsoft.Json;
using VerifyTests;

class FileInfoConverter :
    WriteOnlyJsonConverter<FileInfo>
{
    public override void WriteJson(
        VerifyJsonTextWriter writer,
        FileInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString().Replace('\\','/'));
    }
}