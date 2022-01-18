using System.Linq.Expressions;
using Newtonsoft.Json;
using VerifyTests;

class ExpressionConverter :
    WriteOnlyJsonConverter<Expression>
{
    public override void WriteJson(
        VerifyJsonWriter writer,
        Expression value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}