class ConstructorInfoConverter :
    WriteOnlyJsonConverter<ConstructorInfo>
{
    public override void Write(VerifyJsonWriter writer, ConstructorInfo value) =>
        writer.WriteRawValue(value.SimpleName());
}