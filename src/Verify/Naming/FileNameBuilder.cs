static class FileNameBuilder
{
    public static (string receivedPrefix, string verifiedPrefix) Build(
        string methodName,
        string typeName,
        VerifySettings settings,
        List<string> methodParameters,
        PathInfo pathInfo)
    {
        var (uniquenessReceived, uniquenessVerified) = PrefixUnique.GetUniqueness(settings.Namer);
        if (settings.fileName is not null)
        {
            return (
                settings.fileName + uniquenessReceived,
                settings.fileName + uniquenessVerified);
        }

        var resolvedTypeName = settings.typeName ?? pathInfo.TypeName ?? typeName;
        var resolvedMethodName = settings.methodName ?? pathInfo.MethodName ?? methodName;
        var typeAndMethod = (string) $"{resolvedTypeName}.{resolvedMethodName}";
        var parameterText = GetParameterText(methodParameters, settings);

        if (settings.ignoreParametersForVerified)
        {
            return (
                $"{typeAndMethod}{parameterText}{uniquenessReceived}",
                $"{typeAndMethod}{uniquenessVerified}");
        }

        return (
            $"{typeAndMethod}{parameterText}{uniquenessReceived}",
            $"{typeAndMethod}{parameterText}{uniquenessVerified}");
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