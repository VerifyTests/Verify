class TypeJsonConverter :
    WriteOnlyJsonConverter<Type>
{
    public override void Write(VerifyJsonWriter writer, Type value) =>
        writer.WriteRawValueIfNoStrict(value.SimpleName());
}