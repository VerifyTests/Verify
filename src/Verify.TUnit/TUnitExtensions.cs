static class TUnitExtensions
{
    public static IReadOnlyList<string>? GetParameterNames(this TestDetails details)
    {
        var methodParameterNames = details.TestMethod.Parameters.Select(x => x.Name).ToList();

        var constructorParameterNames = GetConstructorParameterNames(details);
        if (methodParameterNames.Count is 0)
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
        var parameters = details.TestClass.Parameters;
        if (parameters.Length is 0)
        {
            return [];
        }

        return parameters.Select(_ => _.Name).ToList();

    }
}