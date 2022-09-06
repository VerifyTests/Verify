class ExpressionConverter :
    WriteOnlyJsonConverter<Expression>
{
    public override void Write(VerifyJsonWriter writer, Expression value) =>
        writer.WriteRawValueIfNoStrict(value.ToString());
}