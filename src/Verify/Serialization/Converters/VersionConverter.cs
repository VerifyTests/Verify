using Newtonsoft.Json;
using VerifyTests;

class VersionConverter :
    WriteOnlyJsonConverter<Version>
{
    public override void WriteJson(
        VerifyJsonWriter writer,
        Version value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}