static class ReflectionFileNameBuilder
{
    public static (string receivedFileNamePrefix, string verifiedFileNamePrefix, string? directory) FileNamePrefix(
        MethodInfo method,
        Type type,
        string sourceFile,
        VerifySettings settings,
        string uniquenessForReceived,
        string uniquenessForVerified)
    {
        var methodParameters = method.ParameterNames();
        var nameWithParent = type.NameWithParent();
        var methodName = method.Name;

        var pathInfo = VerifierSettings.GetPathInfo(sourceFile, type, method, methodName,nameWithParent);
        var directory = settings.Directory ?? pathInfo.Directory;

        if (settings.fileName is not null)
        {
            return (
                settings.fileName + uniquenessForReceived,
                settings.fileName + uniquenessForVerified,
                directory);
        }

        var resolvedTypeName = settings.typeName ?? pathInfo.TypeName ?? nameWithParent;
        var resolvedMethodName = settings.methodName ?? pathInfo.MethodName ?? methodName;
        var typeAndMethod = (string) $"{resolvedTypeName}.{resolvedMethodName}";
        var parameterText = GetParameterText(methodParameters, settings);

        if (settings.ignoreParametersForVerified)
        {
            return (
                $"{typeAndMethod}{parameterText}{uniquenessForReceived}",
                $"{typeAndMethod}{uniquenessForVerified}",
                directory);
        }

        return (
            $"{typeAndMethod}{parameterText}{uniquenessForReceived}",
            $"{typeAndMethod}{parameterText}{uniquenessForVerified}",
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