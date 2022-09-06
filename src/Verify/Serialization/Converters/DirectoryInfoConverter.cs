class DirectoryInfoConverter :
    WriteOnlyJsonConverter<DirectoryInfo>
{
    public override void Write(VerifyJsonWriter writer, DirectoryInfo value) =>
        writer.WriteRawValueIfNoStrict(value.ToString().Replace('\\', '/'));
}