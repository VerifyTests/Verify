class VersionConverter :
    WriteOnlyJsonConverter<Version>
{
    public override void Write(VerifyJsonWriter writer, Version value)
    {
        writer.WriteValue(value.ToString());
    }
}