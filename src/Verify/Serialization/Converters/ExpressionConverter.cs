using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using VerifyTests;

class ExpressionConverter :
    WriteOnlyJsonConverter<Expression>
{
    public override void WriteJson(
        JsonWriter writer,
        Expression value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.ToString());
    }
}