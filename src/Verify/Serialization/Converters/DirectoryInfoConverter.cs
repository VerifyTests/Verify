class DirectoryInfoConverter :
    WriteOnlyJsonConverter<DirectoryInfo>
{
    public override void Write(VerifyJsonWriter writer, DirectoryInfo value)
    {
        writer.WriteValue(value.ToString().Replace('\\','/'));
    }
}