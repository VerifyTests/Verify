#if !NET462
static class TupleConverter
{
    public static Dictionary<string, object?> ExpressionToDictionary(Expression<Func<ITuple>> expression)
    {
        var unary = (UnaryExpression) expression.Body;
        var methodCall = (MethodCallExpression) unary.Operand;
        var attribute = ReadTupleElementNamesAttribute(methodCall.Method);
        var transforms = attribute.TransformNames;
        var result = expression
            .Compile()
            .Invoke();

        // For 8+ element tuples TransformNames carries trailing nulls for the
        // compiler-synthesized Rest slots; the leaf names come first, aligned
        // with the flattened ITuple indexer. A nested named tuple instead
        // flattens to more names than the tuple has elements, which cannot be
        // mapped positionally.
        var length = result.Length;
        if (transforms.Count(_ => _ is not null) > length)
        {
            throw new("Nested named tuples cannot be used. Use a flat tuple where every part is named.");
        }

        var dictionary = new Dictionary<string, object?>(length);
        for (var index = 0; index < length; index++)
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
#endif