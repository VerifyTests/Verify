namespace VerifyMSTest;

/// <summary>
/// Resolves a <see cref="Type" /> from the class name that MSTest exposes on
/// <c>TestContext.FullyQualifiedTestClassName</c>.
/// </summary>
static class TestClassResolver
{
    static ConcurrentDictionary<string, Type[]> candidates = new(StringComparer.Ordinal);

    public static Type Resolve(string className, string? methodName)
    {
        var types = candidates.GetOrAdd(className, Find);

        if (types.Length == 1)
        {
            return types[0];
        }

        if (types.Length == 0)
        {
            throw new($"Could not resolve the test class `{className}` in any loaded assembly.");
        }

        return Narrow(types, className, methodName);
    }

    /// <summary>
    /// More than one loaded assembly declares the name. Only a test host that runs several test
    /// assemblies in one process can get here, and the running test method is the only thing left
    /// to tell the candidates apart.
    /// </summary>
    static Type Narrow(Type[] types, string className, string? methodName)
    {
        Type? match = null;
        if (methodName is not null)
        {
            foreach (var type in types)
            {
                if (!HasMethod(type, methodName))
                {
                    continue;
                }

                if (match is not null)
                {
                    match = null;
                    break;
                }

                match = type;
            }
        }

        if (match is not null)
        {
            return match;
        }

        var assemblies = string.Join(", ", types.Select(_ => $"`{_.Assembly.GetName().Name}`"));
        throw new(
            $"""
             Found `{className}` in more than one loaded assembly: {assemblies}.
             Rename one of them, or run those test assemblies in separate processes, so the test class can be resolved.
             """);
    }

    static bool HasMethod(Type type, string methodName)
    {
        foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (method.Name == methodName)
            {
                return true;
            }
        }

        return false;
    }

    static Type[] Find(string className)
    {
        List<Type>? found = null;

        // The name is a reflection metadata name, so a nested class arrives as `Outer+Nested`.
        // Type.GetType is not usable here: it probes the calling assembly, which is
        // Verify.MSTest, and never the assembly the test lives in.
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
            {
                continue;
            }

            var type = assembly.GetType(className, throwOnError: false);
            if (type is null)
            {
                continue;
            }

            found ??= [];
            found.Add(type);
        }

        return found?.ToArray() ?? [];
    }
}
