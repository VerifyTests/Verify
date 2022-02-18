class MethodInfoConverter :
    WriteOnlyJsonConverter<MethodInfo>
{
    public override void Write(VerifyJsonWriter writer, MethodInfo value)
    {
        writer.WriteValue(value.SimpleName());
    }
}