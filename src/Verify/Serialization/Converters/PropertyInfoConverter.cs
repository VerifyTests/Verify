class PropertyInfoConverter :
    WriteOnlyJsonConverter<PropertyInfo>
{
    public override void Write(VerifyJsonWriter writer, PropertyInfo value) =>
        writer.WriteRawValue(value.SimpleName());
}