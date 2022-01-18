using Newtonsoft.Json;
using VerifyTests;

class DirectoryInfoConverter :
    WriteOnlyJsonConverter<DirectoryInfo>
{
    public override void WriteJson(
        VerifyJsonTextWriter writer,
        DirectoryInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString().Replace('\\','/'));
    }
}