using Newtonsoft.Json;
using VerifyTests;

class DirectoryInfoConverter :
    WriteOnlyJsonConverter<DirectoryInfo>
{
    public override void Write(
        VerifyJsonWriter writer,
        DirectoryInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString().Replace('\\','/'));
    }
}