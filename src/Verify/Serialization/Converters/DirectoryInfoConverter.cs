class DirectoryInfoConverter :
    WriteOnlyJsonConverter<DirectoryInfo>
{
    public override void Write(VerifyJsonWriter writer, DirectoryInfo value) =>
        writer.WriteSingleLineNoScrubbing(value.ToString().Replace('\\', '/'));
}