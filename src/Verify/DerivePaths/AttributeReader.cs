namespace VerifyTests;

public static class AttributeReader
{
    public static string GetTargetFrameworks() =>
        GetTargetFrameworks(Assembly.GetCallingAssembly());

    public static string GetTargetFrameworks(Assembly assembly) =>
        GetValue(assembly, "Verify.TargetFrameworks");

    public static bool TryGetTargetFrameworks([NotNullWhen(true)] out string? targetFrameworks) =>
        TryGetTargetFrameworks(Assembly.GetCallingAssembly(), out targetFrameworks);

    public static bool TryGetTargetFrameworks(Assembly assembly, [NotNullWhen(true)] out string? targetFrameworks) =>
        TryGetValue(assembly, "Verify.TargetFrameworks", out targetFrameworks);

    public static string GetProjectDirectory() =>
        GetProjectDirectory(Assembly.GetCallingAssembly());

    public static string GetProjectDirectory(Assembly assembly) =>
        GetValue(assembly, "Verify.ProjectDirectory", true);

    public static bool TryGetProjectDirectory([NotNullWhen(true)] out string? projectDirectory) =>
        TryGetProjectDirectory(Assembly.GetCallingAssembly(), true, out projectDirectory);

    public static bool TryGetProjectDirectory(Assembly assembly, [NotNullWhen(true)] out string? projectDirectory) =>
        TryGetProjectDirectory(assembly, true, out projectDirectory);

    public static string GetSolutionDirectory() =>
        GetSolutionDirectory(Assembly.GetCallingAssembly());

    public static string GetSolutionDirectory(Assembly assembly) =>
        GetValue(assembly, "Verify.SolutionDirectory", true);

    public static bool TryGetSolutionDirectory([NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetSolutionDirectory(Assembly.GetCallingAssembly(), true, out solutionDirectory);

    public static bool TryGetSolutionDirectory(Assembly assembly, [NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetSolutionDirectory(assembly, true, out solutionDirectory);

    internal static bool TryGetSolutionDirectory(bool mapPathForVirtualizedRun, [NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetSolutionDirectory(Assembly.GetCallingAssembly(), mapPathForVirtualizedRun, out solutionDirectory);

    internal static bool TryGetSolutionDirectory(Assembly assembly, bool mapPathForVirtualizedRun, [NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetValue(assembly, "Verify.SolutionDirectory", out solutionDirectory, mapPathForVirtualizedRun);

    internal static bool TryGetProjectDirectory(Assembly assembly, bool mapPathForVirtualizedRun, [NotNullWhen(true)] out string? projectDirectory) =>
        TryGetValue(assembly, "Verify.ProjectDirectory", out projectDirectory, mapPathForVirtualizedRun);

    static bool TryGetValue(Assembly assembly, string key, [NotNullWhen(true)] out string? value, bool isSourcePath = false)
    {
        value = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(_ => _.Key == key)
            ?.Value;
        if (isSourcePath)
        {
            value = value == null ? null : IoHelpers.GetMappedBuildPath(value, assembly);
        }

        return value is not null;
    }

    static string GetValue(Assembly assembly, string key, bool isSourcePath = false)
    {
        if (TryGetValue(assembly, key, out var value, isSourcePath))
        {
            return value;
        }

        throw new($"Could not find a `AssemblyMetadataAttribute` named `{key}`.");
    }
}