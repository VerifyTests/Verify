static class FileNameBuilder
{
    public static FrameworkNameVersion? FrameworkName(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        if (attribute is null)
        {
            return null;
        }

        var frameworkName = new FrameworkName(attribute.FrameworkName);
        var name = Namer.GetSimpleFrameworkName(frameworkName);
        var version = frameworkName.Version;
        return new(name, $"{name}{version.Major}_{version.Minor}");
    }

    public static string GetTypeAndMethod(string method, string type, VerifySettings settings, PathInfo pathInfo)
    {
        var resolvedType = settings.typeName ?? pathInfo.TypeName ?? type;
        var resolvedMethod = settings.methodName ?? pathInfo.MethodName ?? method;
        return $"{resolvedType}.{resolvedMethod}";
    }

    public static (string receivedParameters, string verifiedParameters) GetParameterText(IReadOnlyList<string>? methodParameters, VerifySettings settings)
    {
        if (settings.parametersText is not null)
        {
            var parameterText = $"_{settings.parametersText}";
            return (parameterText, parameterText);
        }

        if (methodParameters is null ||
            !settings.TryGetParameters(out var settingsParameters))
        {
            return (string.Empty, string.Empty);
        }

        var numberOfMethodParameters = methodParameters.Count;

        var ignoredParameters = settings.ignoredParameters;
        if (ignoredParameters?.All(methodParameters.Contains) == false)
        {
            throw new($"Some of the ignored parameter names ({string.Join(", ", ignoredParameters)}) do not exist in the test method parameters ({string.Join(", ", methodParameters)}).");
        }

        if (settingsParameters.Length > numberOfMethodParameters)
        {
            throw new($"The number of passed in parameters ({settingsParameters.Length}) must not exceed the number of parameters for the method ({methodParameters.Count}).");
        }

        var allValues = methodParameters
            .Zip(settingsParameters, (name, value) => new KeyValuePair<string, object?>(name, value))
            .ToArray();

        var verifiedValues = ignoredParameters is null
            ? allValues
            : ignoredParameters.Count == 0 ? [] : allValues.Where(x => !ignoredParameters.Contains(x.Key)).ToArray();

        return (BuildParameterString(allValues), BuildParameterString(verifiedValues));

        string BuildParameterString(KeyValuePair<string, object?>[] values)
        {
            var builder = values.Aggregate(new StringBuilder(), (acc, seed) =>
            {
                acc.Append('_');
                acc.Append(seed.Key);
                acc.Append('=');
                VerifierSettings.AppendParameter(seed.Value, acc, true);
                return acc;
            });

            var parameterText = builder.ToString();

            if (settings.hashParameters || VerifierSettings.hashParameters)
            {
                return HashString(parameterText);
            }

            return parameterText;
        }
    }

    static string HashString(string value)
    {
        var data = XxHash64.Hash(Encoding.UTF8.GetBytes(value));

        var builder = new StringBuilder("_", 17);

        foreach (var item in data)
        {
            builder.Append(item.ToString("x2"));
        }

        return builder.ToString();
    }
}