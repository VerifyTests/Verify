class ExpressionConverter :
    WriteOnlyJsonConverter<Expression>
{
    public override void Write(VerifyJsonWriter writer, Expression value)
    {
        writer.WriteValue(value.ToString());
    }
}