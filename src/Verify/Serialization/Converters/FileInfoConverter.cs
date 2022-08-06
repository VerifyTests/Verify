class FileInfoConverter :
    WriteOnlyJsonConverter<FileInfo>
{
    public override void Write(VerifyJsonWriter writer, FileInfo value) =>
        writer.WriteSingleLineNoScrubbing(value.ToString().Replace('\\', '/'));
}