class FileInfoConverter :
    WriteOnlyJsonConverter<FileInfo>
{
    public override void Write(VerifyJsonWriter writer, FileInfo value) =>
        writer.WriteRawValueIfNoStrict(value.ToString().Replace('\\', '/'));
}