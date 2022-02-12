class TypeJsonConverter :
    WriteOnlyJsonConverter<Type>
{
    public override void Write(VerifyJsonWriter writer, Type value)
    {
        writer.WriteValue(value.SimpleName());
    }
}