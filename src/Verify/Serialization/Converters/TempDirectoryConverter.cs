class TempDirectoryConverter :
    WriteOnlyJsonConverter<TempDirectory>
{
    public override void Write(VerifyJsonWriter writer, TempDirectory value) =>
        writer.WriteRawValueIfNoStrict("{TempDirectory}");
}