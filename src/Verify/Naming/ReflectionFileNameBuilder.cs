class ReflectionFileNameBuilder
{
    public static (string receivedFileNamePrefix, string verifiedFileNamePrefix, string? directory) FileNamePrefix(
        MethodInfo method,
        Type type,
        string sourceFile,
        VerifySettings settings,
        string uniqueness)
    {
        var pathInfo = VerifierSettings.GetPathInfo(sourceFile, type, method);
        var directory = settings.Directory ?? pathInfo.Directory;

        if (settings.fileName is not null)
        {
            var filename = settings.fileName + uniqueness;
            return (filename, filename, directory);
        }

        var typeName = settings.typeName ?? pathInfo.TypeName ?? GetTypeName(type);
        var methodName = settings.methodName ?? pathInfo.MethodName ?? method.Name;

        var parameterText = GetParameterText(method, settings);

        var withParameters = $"{typeName}.{methodName}{parameterText}{uniqueness}";

        if (settings.ignoreParametersForVerified)
        {
            var withoutParameters = $"{typeName}.{methodName}{uniqueness}";
            return (withParameters, withoutParameters, directory);
        }

        return (withParameters, withParameters, directory);
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

    static string GetTypeName(Type type)
    {
        if (type.IsNested)
        {
            return $"{type.ReflectedType!.Name}.{type.Name}";
        }

        return type.Name;
    }
}