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

        var (prefixWithParameters, prefixWithoutParameters) = GetFileNamePrefix(method, type, settings, pathInfo, uniqueness);

        var receivedFilePrefix = prefixWithParameters;
        var verifiedFilePrefix = settings.ignoreParametersForVerified ? prefixWithoutParameters : prefixWithParameters;

        var directory = settings.Directory ?? pathInfo.Directory;
        return (receivedFilePrefix, verifiedFilePrefix, directory);
    }

    static (string prefixWithParameters, string prefixWithoutParameters) GetFileNamePrefix(MethodInfo method, Type type, VerifySettings settings, PathInfo pathInfo, string uniqueness)
    {
        if (settings.fileName is not null)
        {
            var filename = settings.fileName + uniqueness;
            return (filename, filename);
        }

        var typeName = settings.typeName ?? pathInfo.TypeName ?? GetTypeName(type);
        var methodName = settings.methodName ?? pathInfo.MethodName ?? method.Name;

        var parameterText = GetParameterText(method, settings);

        var withParameters = $"{typeName}.{methodName}{parameterText}{uniqueness}";
        var withoutParameters = $"{typeName}.{methodName}{uniqueness}";
        return (withParameters, withoutParameters);
    }

    static string GetParameterText(MethodInfo method, VerifySettings settings)
    {
        if (settings.parametersText is not null)
        {
            return $"_{settings.parametersText}";
        }

        var methodParameters = method.GetParameters();
        if (methodParameters.IsEmpty())
        {
            return "";
        }

        var settingsParameters = settings.parameters;
        if (settingsParameters is null)
        {
            throw BuildMissingParametersException(method, methodParameters);
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

    static Exception BuildMissingParametersException(MethodInfo method, ParameterInfo[] parameters)
    {
        var names = string.Join(", ", parameters.Select(x => x.Name));
        return new($@"Method `{method.DeclaringType!.Name}.{method.Name}` requires parameters, but none have been defined. Add UseParameters. For example:

var settings = new VerifySettings();
settings.UseParameters({names});
await Verifier.Verify(target, settings);

or

await Verifier.Verify(target).UseParameters({names});
");
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