static class ReflectionFileNameBuilder
{
    public static (string receivedPrefix, string verifiedPrefix, string? directory) FileNamePrefix(
        MethodInfo method,
        Type type,
        string sourceFile,
        VerifySettings settings,
        string uniquenessReceived,
        string uniquenessVerified)
    {
        var methodParameters = method.ParameterNames();
        var nameWithParent = type.NameWithParent();
        var methodName = method.Name;

        return FileNamePrefix(methodName, nameWithParent, sourceFile, settings, uniquenessReceived, uniquenessVerified, methodParameters);
    }

    public static (string receivedPrefix, string verifiedPrefix, string? directory) FileNamePrefix(
        string methodName,
        string typeName,
        string sourceFile,
        VerifySettings settings,
        string uniquenessReceived,
        string uniquenessVerified,
        List<string> methodParameters)
    {
        var pathInfo = VerifierSettings.GetPathInfo(sourceFile, typeName, methodName);
        var directory = settings.Directory ?? pathInfo.Directory;

        if (settings.fileName is not null)
        {
            return (
                settings.fileName + uniquenessReceived,
                settings.fileName + uniquenessVerified,
                directory);
        }

        var resolvedTypeName = settings.typeName ?? pathInfo.TypeName ?? typeName;
        var resolvedMethodName = settings.methodName ?? pathInfo.MethodName ?? methodName;
        var typeAndMethod = (string) $"{resolvedTypeName}.{resolvedMethodName}";
        var parameterText = GetParameterText(methodParameters, settings);

        if (settings.ignoreParametersForVerified)
        {
            return (
                $"{typeAndMethod}{parameterText}{uniquenessReceived}",
                $"{typeAndMethod}{uniquenessVerified}",
                directory);
        }

        return (
            $"{typeAndMethod}{parameterText}{uniquenessReceived}",
            $"{typeAndMethod}{parameterText}{uniquenessVerified}",
            directory);
    }

    static string GetParameterText(List<string> methodParameters, VerifySettings settings)
    {
        if (settings.parametersText is not null)
        {
            return $"_{settings.parametersText}";
        }

        var settingsParameters = settings.parameters;
        if (methodParameters.IsEmpty() || settingsParameters is null)
        {
            return "";
        }

        if (settingsParameters.Length > methodParameters.Count)
        {
            throw new($"The number of passed in parameters ({settingsParameters.Length}) must be fewer than the number of parameters for the method ({methodParameters.Count}).");
        }

        var dictionary = new Dictionary<string, object?>();
        for (var index = 0; index < settingsParameters.Length; index++)
        {
            var parameter = methodParameters[index];
            var value = settingsParameters[index];
            dictionary[parameter] = value;
        }

        var concat = ParameterBuilder.Concat(dictionary);
        return $"_{concat}";
    }
}