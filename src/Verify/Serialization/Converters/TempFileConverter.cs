class TempFileConverter :
    WriteOnlyJsonConverter<TempFile>
{
    public override void Write(VerifyJsonWriter writer, TempFile value) =>
        writer.WriteRawValueIfNoStrict("{TempFile}");
}