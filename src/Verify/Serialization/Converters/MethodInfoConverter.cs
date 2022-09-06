class MethodInfoConverter :
    WriteOnlyJsonConverter<MethodInfo>
{
    public override void Write(VerifyJsonWriter writer, MethodInfo value) =>
        writer.WriteRawValueIfNoStrict(value.SimpleName());
}