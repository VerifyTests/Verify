static class TUnitExtensions
{
    public static IReadOnlyList<string>? GetParameterNames(this TestDetails details)
    {
        var methodParameterNames = details.MethodInfo.ParameterNames();

        var constructorParameterNames = GetConstructorParameterNames(details);
        if (methodParameterNames == null)
        {
            if (constructorParameterNames.Count == 0)
            {
                return null;
            }

            return constructorParameterNames;
        }

        if (constructorParameterNames.Count == 0 &&
            methodParameterNames.Count == 0)
        {
            return null;
        }

        return [.. constructorParameterNames, .. methodParameterNames];
    }

    static List<string> GetConstructorParameterNames(TestDetails details)
    {
        var constructors = details.ClassType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        if (constructors.Length <= 1)
        {
            return constructors[0].GetParameters().Select(_ => _.Name!).ToList();
        }

        throw new("Found multiple constructors. Unable to derive names of parameters. Instead use UseParameters to pass in explicit parameter.");
    }
}