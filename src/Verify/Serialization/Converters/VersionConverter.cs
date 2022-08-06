class VersionConverter :
    WriteOnlyJsonConverter<Version>
{
    public override void Write(VerifyJsonWriter writer, Version value) =>
        writer.WriteRawValue(value.ToString());
}