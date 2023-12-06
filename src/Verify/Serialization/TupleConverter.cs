static class TupleConverter
{
    public static Dictionary<string, object?> ExpressionToDictionary(Expression<Func<ITuple>> expression)
    {
        var unary = (UnaryExpression) expression.Body;
        var methodCall = (MethodCallExpression) unary.Operand;
        var attribute = ReadTupleElementNamesAttribute(methodCall.Method);
        var transforms = attribute.TransformNames;
        var dictionary = new Dictionary<string, object?>(transforms.Count);
        var result = expression
            .Compile()
            .Invoke();
        for (var index = 0; index < transforms.Count; index++)
        {
            var transform = transforms[index];
            if (transform is null)
            {
                throw new("Only tuples with all parts are named can be used.");
            }

            dictionary.Add(transform, result[index]);
        }

        return dictionary;
    }

    static TupleElementNamesAttribute ReadTupleElementNamesAttribute(MethodInfo method)
    {
        var attributes = method.ReturnTypeCustomAttributes
            .GetCustomAttributes(typeof(TupleElementNamesAttribute), false);

        if (attributes.Length == 0)
        {
            throw new("Verify is only to be used on methods that return a tuple.");
        }

        return (TupleElementNamesAttribute) attributes[0];
    }
}