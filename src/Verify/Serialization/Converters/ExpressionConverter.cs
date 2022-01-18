using System.Linq.Expressions;
using Newtonsoft.Json;
using VerifyTests;

class ExpressionConverter :
    WriteOnlyJsonConverter<Expression>
{
    public override void WriteJson(
        VerifyJsonTextWriter writer,
        Expression value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}