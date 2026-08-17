namespace VerifyMSTest;

public record TestExecutionContext(TestContext TestContext, Type TestClass)
{
    public Assembly Assembly { get; } = TestClass.Assembly;

    MethodInfo? method;

    // Resolved lazily: the method scan is only needed when a Verify call actually
    // builds a verifier, not for every test that constructs a context.
    public MethodInfo Method => method ??= FindMethod(TestClass, TestContext);

    static MethodInfo FindMethod(Type type, TestContext context)
    {
        var testName = context.TestName;

        if (testName is null)
        {
            throw new("Expected TestContext.TestName to have a non null value");
        }

        var span = testName.AsSpan();

        MethodInfo? first = null;
        List<MethodInfo>? overloads = null;
        foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!span.SequenceEqual(method.Name))
            {
                continue;
            }

            if (first is null)
            {
                first = method;
                continue;
            }

            overloads ??= [first];
            overloads.Add(method);
        }

        if (first is null)
        {
            throw new($"Could not find method `{type.Name}.{testName}`.");
        }

        if (overloads is null)
        {
            return first;
        }

        // Overloads share a name, so the data the test was invoked with is all there is
        // to tell them apart. Falls back to the first when it does not narrow to one.
        return FindOverload(overloads, context.TestData) ?? first;
    }

    static MethodInfo? FindOverload(List<MethodInfo> overloads, object?[]? data)
    {
        data ??= [];

        List<MethodInfo> byCount = [];
        foreach (var overload in overloads)
        {
            if (MatchesCount(overload, data.Length))
            {
                byCount.Add(overload);
            }
        }

        if (byCount.Count <= 1)
        {
            return byCount.FirstOrDefault();
        }

        MethodInfo? byType = null;
        foreach (var overload in byCount)
        {
            if (!MatchesTypes(overload, data))
            {
                continue;
            }

            if (byType is not null)
            {
                return null;
            }

            byType = overload;
        }

        return byType;
    }

    static bool MatchesCount(MethodInfo method, int dataLength)
    {
        var parameters = method.GetParameters();
        if (parameters.Length == dataLength)
        {
            return true;
        }

        // A params array DataRow exposes raw pre-binding data, so its length only has
        // to cover the parameters that precede the array
        return parameters.Length > 0 &&
               parameters[^1].IsDefined(typeof(ParamArrayAttribute)) &&
               dataLength >= parameters.Length - 1;
    }

    static bool MatchesTypes(MethodInfo method, object?[] data)
    {
        var parameters = method.GetParameters();
        if (parameters.Length != data.Length)
        {
            return false;
        }

        for (var index = 0; index < parameters.Length; index++)
        {
            var type = parameters[index].ParameterType;
            var value = data[index];

            if (value is null)
            {
                if (type.IsValueType &&
                    Nullable.GetUnderlyingType(type) is null)
                {
                    return false;
                }

                continue;
            }

            if (!type.IsInstanceOfType(value))
            {
                return false;
            }
        }

        return true;
    }
}