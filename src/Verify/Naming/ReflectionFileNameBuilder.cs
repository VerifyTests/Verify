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
        var pathInfo = VerifierSettings.GetPathInfo(sourceFile, type, method);
        var directory = settings.Directory ?? pathInfo.Directory;

        if (settings.fileName is not null)
        {
            return (
                settings.fileName + uniquenessReceived,
                settings.fileName + uniquenessVerified,
                directory);
        }

        var typeAndMethod = GetTypeAndMethod(method, type, settings, pathInfo);
        var parameterText = GetParameterText(method, settings);

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

    static string GetTypeAndMethod(MethodInfo method, Type type, VerifySettings settings, PathInfo pathInfo)
    {
        var typeName = settings.typeName ?? pathInfo.TypeName ?? type.NameWithParent();
        var methodName = settings.methodName ?? pathInfo.MethodName ?? method.Name;

        return $"{typeName}.{methodName}";
    }

    static string GetParameterText(MethodInfo method, VerifySettings settings)
    {
        if (settings.parametersText is not null)
        {
            return $"_{settings.parametersText}";
        }

        var methodParameters = method.GetParameters();
        var settingsParameters = settings.parameters;
        if (methodParameters.IsEmpty() || settingsParameters is null)
        {
            return "";
        }

        if (settingsParameters.Length > methodParameters.Length)
        {
            throw new($"The number of passed in parameters ({settingsParameters.Length}) must be fewer than the number of parameters for the method ({methodParameters.Length}).");
        }

        var dictionary = new Dictionary<string, object?>();
        for (var index = 0; index < settingsParameters.Length; index++)
        {
            var parameter = methodParameters[index];
            var value = settingsParameters[index];
            dictionary[parameter.Name!] = value;
        }

        var concat = ParameterBuilder.Concat(dictionary);
        return $"_{concat}";
    }
}