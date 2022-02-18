class ReflectionFileNameBuilder
{
    public static (string fileNamePrefix, string? directory) FileNamePrefix(
        MethodInfo method,
        Type type,
        string sourceFile,
        VerifySettings settings,
        string uniqueness)
    {
        var pathInfo = VerifierSettings.GetPathInfo(sourceFile, type, method);

        var fileNamePrefix = GetFileNamePrefix(method, type, settings, pathInfo, uniqueness);
        var directory = settings.Directory ?? pathInfo.Directory;
        return (fileNamePrefix, directory);
    }

    static string GetFileNamePrefix(MethodInfo method, Type type, VerifySettings settings, PathInfo pathInfo, string uniqueness)
    {
        if (settings.fileName is not null)
        {
            return settings.fileName + uniqueness;
        }

        var typeName = settings.typeName ?? pathInfo.TypeName ?? GetTypeName(type);
        var methodName = settings.methodName ?? pathInfo.MethodName ?? method.Name;

        var parameterText = GetParameterText(method, settings);

        return $"{typeName}.{methodName}{parameterText}{uniqueness}";
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

        if (methodParameters.Length != settingsParameters.Length)
        {
            throw new($"The number of passed in parameters ({settingsParameters.Length}) must match the number of parameters for the method ({methodParameters.Length}).");
        }

        var dictionary = new Dictionary<string, object?>();
        for (var index = 0; index < methodParameters.Length; index++)
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