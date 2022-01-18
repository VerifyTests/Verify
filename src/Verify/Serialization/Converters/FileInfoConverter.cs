using Newtonsoft.Json;
using VerifyTests;

class FileInfoConverter :
    WriteOnlyJsonConverter<FileInfo>
{
    public override void Write(
        VerifyJsonWriter writer,
        FileInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString().Replace('\\','/'));
    }
}