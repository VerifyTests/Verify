static class TargetAssembly
{
    static Assembly? assembly;
    public static string ProjectDir { get; private set; } = null!;
    public static string? SolutionDir { get; private set; }
    public static bool TargetsMultipleFramework { get; private set; } = true;

    public static void Assign(Assembly assembly)
    {
        if (TargetAssembly.assembly is not null)
        {
            return;
        }

        Namer.UseAssembly(assembly);
        IoHelpers.MapPathsForCallingAssembly(assembly);
        ProjectDir = AttributeReader.GetProjectDirectory(assembly);
        AttributeReader.TryGetSolutionDirectory(assembly, out var solutionDir);
        SolutionDir = solutionDir;
        if (AttributeReader.TryGetTargetFrameworks(assembly, out var targetFrameworks))
        {
            TargetsMultipleFramework = targetFrameworks.Contains(';');
        }

        SolutionDir = solutionDir;
        ApplyScrubbers.UseAssembly(solutionDir, ProjectDir);
        TargetAssembly.assembly = assembly;
    }
}