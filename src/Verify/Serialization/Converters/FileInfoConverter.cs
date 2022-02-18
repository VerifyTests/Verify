class FileInfoConverter :
    WriteOnlyJsonConverter<FileInfo>
{
    public override void Write(VerifyJsonWriter writer, FileInfo value)
    {
        writer.WriteValue(value.ToString().Replace('\\','/'));
    }
}