using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using VerifyTests;

class ExpressionConverter :
    WriteOnlyJsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            return;
        }

        var expression = (Expression) value;
        writer.WriteValue(expression.ToString());
    }

    public override bool CanConvert(Type type)
    {
        return typeof(Expression).IsAssignableFrom(type);
    }
}