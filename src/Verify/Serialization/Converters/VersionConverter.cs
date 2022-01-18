using Newtonsoft.Json;
using VerifyTests;

class VersionConverter :
    WriteOnlyJsonConverter<Version>
{
    public override void Write(
        VerifyJsonWriter writer,
        Version value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}