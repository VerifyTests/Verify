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
        type = settings.TypeName ?? pathInfo.TypeName ?? type;
        method = settings.MethodName ?? pathInfo.MethodName ?? method;
        return $"{type}.{method}";
    }

    public static (string receivedParameters, string verifiedParameters) GetParameterText(IReadOnlyList<string>? methodParameters, VerifySettings settings, Counter counter)
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

        if (settingsParameters.Length > numberOfMethodParameters)
        {
            throw new($"The number of passed in parameters ({settingsParameters.Length}) must not exceed the number of parameters for the method ({methodParameters.Count}).");
        }

        var allValues = methodParameters
            .Zip(settingsParameters, (name, value) => new KeyValuePair<string, object?>(name, value))
            .ToArray();

        var ignored = settings.ignoredParameters;
        if (ignored?.All(methodParameters.Contains) == false)
        {
            throw new($"Some of the ignored parameter names ({string.Join(", ", ignored)}) do not exist in the test method parameters ({string.Join(", ", methodParameters)}).");
        }

        var verifiedValues = GetVerifiedValues(ignored, allValues);

        return (
            BuildParameterString(allValues, counter),
            BuildParameterString(verifiedValues, counter));
    }

    static IEnumerable<KeyValuePair<string, object?>> GetVerifiedValues(HashSet<string>? ignored, KeyValuePair<string, object?>[] allValues)
    {
        if (ignored is null)
        {
            return allValues;
        }

        if (ignored.Count == 0)
        {
            return [];
        }

        return allValues.Where(_ => !ignored.Contains(_.Key));
    }

    static string BuildParameterString(IEnumerable<KeyValuePair<string, object?>> values, Counter counter)
    {
        var builder = values.Aggregate(
            new StringBuilder(),
            (acc, seed) =>
            {
                acc.Append('_');
                acc.Append(seed.Key);
                acc.Append('=');
                VerifierSettings.AppendParameter(seed.Value, acc, true, counter);
                return acc;
            });

        return builder.ToString();
    }
}