static class FileNameBuilder
{
    public static (string receivedPrefix, string verifiedPrefix) Build(
        string methodName,
        string typeName,
        VerifySettings settings,
        List<string> methodParameters,
        PathInfo pathInfo,
        string typeAndMethod,
        string parameterText)
    {
        var builder = PrefixUnique.SharedUniqueness(settings.Namer);

        var verifiedBuilder = new StringBuilder(builder.Length);
        verifiedBuilder.Append(builder);
        PrefixUnique.AppendRuntimeForVerified(settings.Namer, verifiedBuilder);

        var receivedBuilder = new StringBuilder(builder.Length);
        receivedBuilder.Append(builder);
        PrefixUnique.AppendRuntimeForReceived(settings.Namer, receivedBuilder);
        
        var uniquenessReceived = receivedBuilder.ToString();
        var uniquenessVerified = verifiedBuilder.ToString();
        if (settings.fileName is not null)
        {
            return (
                settings.fileName + uniquenessReceived,
                settings.fileName + uniquenessVerified);
        }


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

    public static string GetTypeAndMethod(string method, string type, VerifySettings settings, PathInfo pathInfo)
    {
        var resolvedType = settings.typeName ?? pathInfo.TypeName ?? type;
        var resolvedMethod = settings.methodName ?? pathInfo.MethodName ?? method;
        return $"{resolvedType}.{resolvedMethod}";
    }

    public  static string GetParameterText(List<string> methodParameters, VerifySettings settings)
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