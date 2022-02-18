namespace VerifyTests;

public static class AttributeReader
{
    public static string GetProjectDirectory()
    {
        return GetProjectDirectory(Assembly.GetCallingAssembly());
    }

    public static string GetProjectDirectory(Assembly assembly)
    {
        return GetValue(assembly, "Verify.ProjectDirectory");
    }

    public static bool TryGetProjectDirectory([NotNullWhen(true)] out string? projectDirectory)
    {
        return TryGetProjectDirectory(Assembly.GetCallingAssembly(), out projectDirectory);
    }

    public static bool TryGetProjectDirectory(Assembly assembly, [NotNullWhen(true)] out string? projectDirectory)
    {
        return TryGetValue(assembly, "Verify.ProjectDirectory", out projectDirectory);
    }

    public static string GetSolutionDirectory()
    {
        return GetSolutionDirectory(Assembly.GetCallingAssembly());
    }

    public static string GetSolutionDirectory(Assembly assembly)
    {
        return GetValue(assembly, "Verify.SolutionDirectory");
    }

    public static bool TryGetSolutionDirectory([NotNullWhen(true)] out string? solutionDirectory)
    {
        return TryGetSolutionDirectory(Assembly.GetCallingAssembly(), out solutionDirectory);
    }

    public static bool TryGetSolutionDirectory(Assembly assembly, [NotNullWhen(true)] out string? solutionDirectory)
    {
        return TryGetValue(assembly, "Verify.SolutionDirectory", out solutionDirectory);
    }

    static bool TryGetValue(Assembly assembly, string key, [NotNullWhen(true)] out string? value)
    {
        value = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(x => x.Key == key)
            ?.Value;
        return value != null;
    }

    static string GetValue(Assembly assembly, string key)
    {
        if (TryGetValue(assembly, key, out var value))
        {
            return value;
        }

        throw new($"Could not find a `AssemblyMetadataAttribute` named `{key}`.");
    }
}