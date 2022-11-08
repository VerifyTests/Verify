#if !NETSTANDARD2_0 && !NET462
static class TupleConverter
{
    public static Dictionary<string, object?> ExpressionToDictionary(Expression<Func<ITuple>> expression)
    {
        var unaryExpression = (UnaryExpression) expression.Body;
        var methodCallExpression = (MethodCallExpression) unaryExpression.Operand;
        var method = methodCallExpression.Method;
        var attribute = ReadTupleElementNamesAttribute(method);
        var dictionary = new Dictionary<string, object?>();
        var result = expression.Compile().Invoke();
        for (var index = 0; index < attribute.TransformNames.Count; index++)
        {
            var transformName = attribute.TransformNames[index];
            if (transformName is null)
            {
                throw new("Only tuples with all parts are named can be used.");
            }

            dictionary.Add(transformName, result[index]);
        }

        return dictionary;
    }

    static TupleElementNamesAttribute ReadTupleElementNamesAttribute(MethodInfo method)
    {
        var attribute = (TupleElementNamesAttribute?) method.ReturnTypeCustomAttributes
            .GetCustomAttributes(typeof(TupleElementNamesAttribute), false)
            .SingleOrDefault();
        if (attribute is not null)
        {
            return attribute;
        }

        throw new("Verify is only to be used on methods that return a tuple.");
    }
}
#endif