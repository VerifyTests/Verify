static class Extensions
{
    public static IReadOnlyList<string>? GetParameterNames(this TestDetails details)
    {
        var methodParameterNames = details.MethodInfo.ParameterNames();

        var names = GetConstructorParameterNames(details.ClassType, details.TestClassArguments.Length);
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
