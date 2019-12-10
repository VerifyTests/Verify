#if !NETSTANDARD2_0
using System.Reflection;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;

static class TupleConverter
{
    public static Dictionary<string, object> ExpressionToDictionary(Expression<Func<ITuple>> expression)
    {
        var unaryExpression = (UnaryExpression) expression.Body;
        var methodCallExpression = (MethodCallExpression) unaryExpression.Operand;
        var method = methodCallExpression.Method;
        var attribute = ReadTupleElementNamesAttribute(method);
        var dictionary = new Dictionary<string, object>();
        var result = expression.Compile().Invoke();
        for (var index = 0; index < attribute.TransformNames.Count; index++)
        {
            var transformName = attribute.TransformNames[index];
            dictionary.Add(transformName, result[index]);
        }

        return dictionary;
    }

    static TupleElementNamesAttribute ReadTupleElementNamesAttribute(MethodInfo method)
    {
        var attribute = (TupleElementNamesAttribute) method.ReturnTypeCustomAttributes.GetCustomAttributes(typeof(TupleElementNamesAttribute), false).SingleOrDefault();
        if (attribute != null)
        {
            return attribute;
        }

        throw new Exception("Verify is only to be used on methods that return a tuple.");
    }
}
#endif