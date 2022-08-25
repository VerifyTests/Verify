﻿namespace VerifyTests;

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
        GetValue(assembly, "Verify.ProjectDirectory");

    public static bool TryGetProjectDirectory([NotNullWhen(true)] out string? projectDirectory) =>
        TryGetProjectDirectory(Assembly.GetCallingAssembly(), out projectDirectory);

    public static bool TryGetProjectDirectory(Assembly assembly, [NotNullWhen(true)] out string? projectDirectory) =>
        TryGetValue(assembly, "Verify.ProjectDirectory", out projectDirectory);

    public static string GetSolutionDirectory() =>
        GetSolutionDirectory(Assembly.GetCallingAssembly());

    public static string GetSolutionDirectory(Assembly assembly) =>
        GetValue(assembly, "Verify.SolutionDirectory");

    public static bool TryGetSolutionDirectory([NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetSolutionDirectory(Assembly.GetCallingAssembly(), out solutionDirectory);

    public static bool TryGetSolutionDirectory(Assembly assembly, [NotNullWhen(true)] out string? solutionDirectory) =>
        TryGetValue(assembly, "Verify.SolutionDirectory", out solutionDirectory);

    static bool TryGetValue(Assembly assembly, string key, [NotNullWhen(true)] out string? value)
    {
        value = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(_ => _.Key == key)
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