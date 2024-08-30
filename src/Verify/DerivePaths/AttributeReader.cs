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

    public static string GetProjectName() =>
        GetProjectName(Assembly.GetCallingAssembly());

    public static string GetProjectDirectory(Assembly assembly) =>
        GetValue(assembly, "Verify.ProjectDirectory", true);

    public static string GetProjectName(Assembly assembly) =>
        GetValue(assembly, "Verify.ProjectName", true);

    public static bool TryGetProjectDirectory([NotNullWhen(true)] out string? projectDirectory) =>
        TryGetProjectDirectory(Assembly.GetCallingAssembly(), true, out projectDirectory);

    public static bool TryGetProjectName([NotNullWhen(true)] out string? projectName) =>
        TryGetProjectName(Assembly.GetCallingAssembly(), true, out projectName);

    public static bool TryGetProjectDirectory(Assembly assembly, [NotNullWhen(true)] out string? projectDirectory) =>
        TryGetProjectDirectory(assembly, true, out projectDirectory);

    public static bool TryGetProjectName(Assembly assembly, [NotNullWhen(true)] out string? projectName) =>
        TryGetProjectName(assembly, true, out projectName);

    public static string GetSolutionDirectory() =>
        GetSolutionDirectory(Assembly.GetCallingAssembly());

    public static string GetSolutionName() =>
        GetSolutionName(Assembly.GetCallingAssembly());

    public static string GetSolutionDirectory(Assembly assembly) =>
        GetValue(assembly, "Verify.SolutionDirectory", true);

    public static string GetSolutionName(Assembly assembly) =>
        GetValue(assembly, "Verify.SolutionName", true);

    public static bool TryGetSolutionDirectory([NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetSolutionDirectory(Assembly.GetCallingAssembly(), true, out solutionDirectory);

    public static bool TryGetSolutionName([NotNullWhen(true)] out string? solutionName) =>
        TryGetSolutionName(Assembly.GetCallingAssembly(), true, out solutionName);

    public static bool TryGetSolutionDirectory(Assembly assembly, [NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetSolutionDirectory(assembly, true, out solutionDirectory);

    public static bool TryGetSolutionName(Assembly assembly, [NotNullWhen(true)] out string? solutionName) =>
        TryGetSolutionName(assembly, true, out solutionName);

    internal static bool TryGetSolutionDirectory(bool mapPathForVirtualizedRun, [NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetSolutionDirectory(Assembly.GetCallingAssembly(), mapPathForVirtualizedRun, out solutionDirectory);

    internal static bool TryGetSolutionDirectory(Assembly assembly, bool mapPathForVirtualizedRun, [NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetValue(assembly, "Verify.SolutionDirectory", out solutionDirectory, mapPathForVirtualizedRun);

    internal static bool TryGetSolutionName(Assembly assembly, bool mapPathForVirtualizedRun, [NotNullWhen(true)] out string? solutionName) =>
        TryGetValue(assembly, "Verify.SolutionName", out solutionName, mapPathForVirtualizedRun);

    internal static bool TryGetProjectDirectory(Assembly assembly, bool mapPathForVirtualizedRun, [NotNullWhen(true)] out string? projectDirectory) =>
        TryGetValue(assembly, "Verify.ProjectDirectory", out projectDirectory, mapPathForVirtualizedRun);

    internal static bool TryGetProjectName(Assembly assembly, bool mapPathForVirtualizedRun, [NotNullWhen(true)] out string? projectName) =>
        TryGetValue(assembly, "Verify.ProjectName", out projectName, mapPathForVirtualizedRun);

    static bool TryGetValue(Assembly assembly, string key, [NotNullWhen(true)] out string? value, bool isSourcePath = false)
    {
        foreach (var attribute in Attribute.GetCustomAttributes(assembly, typeof(AssemblyMetadataAttribute), false))
        {
            var metaData = (AssemblyMetadataAttribute) attribute;
            if (metaData.Key != key)
            {
                continue;
            }

            value = metaData.Value ??
                    throw new($"Null value for `AssemblyMetadataAttribute` named `{key}`.");
            if (isSourcePath)
            {
                value = IoHelpers.GetMappedBuildPath(value, assembly);
            }

            return true;
        }

        value = null;
        return false;
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