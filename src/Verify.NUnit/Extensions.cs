static class Extensions
{
    public static IMethodInfo GetTestMethod(this TestAdapter adapter)
    {
        var testMethod = adapter.Method;
        if (testMethod is null)
        {
            throw new("TestContext.CurrentContext.Test.Method is null. Verify can only be used from within a test method.");
        }

        return testMethod;
    }

    public static bool TryGetParent(this TestAdapter adapter, [NotNullWhen(true)] out ITest? parent)
    {
        parent = adapter.Parent;
        if (parent is ParameterizedMethodSuite methodSuite)
        {
            parent = methodSuite.Parent;
        }

        return parent != null;
    }

    public static IReadOnlyList<string>? GetParameterNames(this TestAdapter adapter, IReadOnlyList<string>? methodParameterNames)
    {
        var method = adapter.Method!;

        if (!adapter.TryGetParent(out var parent))
        {
            return methodParameterNames;
        }

        var argumentsLength = parent.Arguments.Length;
        if (argumentsLength == 0)
        {
            return methodParameterNames;
        }

        var names = GetConstructorParameterNames(method.TypeInfo.Type, parent.Arguments.Length);
        if (methodParameterNames == null)
        {
            return names.ToList();
        }

        return [.. names, .. methodParameterNames];
    }

    public static IEnumerable<string> GetConstructorParameterNames(this Type type, int argumentsLength)
    {
        IEnumerable<string>? names = null;
        foreach (var constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length != argumentsLength)
            {
                continue;
            }

            if (names != null)
            {
                throw new($"Found multiple constructors with {argumentsLength} parameters. Unable to derive names of parameters. Instead use UseParameters to pass in explicit parameter.");
            }

            names = parameters.Select(_ => _.Name!);
        }

        if (names == null)
        {
            throw new($"Could not find constructor with {argumentsLength} parameters.");
        }

        return names;
    }
}