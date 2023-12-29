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

    public static string GetParameterText(IReadOnlyList<string>? methodParameters, VerifySettings settings)
    {
        if (settings.parametersText is not null)
        {
            return $"_{settings.parametersText}";
        }

        if (methodParameters is null ||
            !settings.TryGetParameters(out var settingsParameters))
        {
            return "";
        }

        if (settingsParameters.Length > methodParameters.Count)
        {
            throw new($"The number of passed in parameters ({settingsParameters.Length}) must be fewer than the number of parameters for the method ({methodParameters.Count}).");
        }

        var builder = new StringBuilder("_");
        for (var index = 0; index < settingsParameters.Length; index++)
        {
            var parameter = methodParameters[index];
            var value = settingsParameters[index];
            builder.Append(parameter);
            builder.Append('=');
            VerifierSettings.AppendParameter(value, builder, true);
            if (index < settingsParameters.Length - 1)
            {
                builder.Append('_');
            }
        }

        var parameterText = builder.ToString();

        if (settings.hashParameters)
        {
            var hashed = HashString(parameterText);
            return $"_{hashed}";
        }

        return parameterText;
    }

    static string HashString(string value)
    {
        var data = XxHash64.Hash(Encoding.UTF8.GetBytes(value));

        var builder = new StringBuilder(16);

        foreach (var item in data)
        {
            builder.Append(item.ToString("x2"));
        }

        return builder.ToString();
    }
}