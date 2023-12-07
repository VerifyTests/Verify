namespace VerifyTests;

public static partial class VerifierSettings
{
    static Assembly? assembly;

    [Experimental("VerifyTestsProjectDir")]
    public static string ProjectDir { get; private set; } = null!;

    internal static string? SolutionDir { get; private set; }
    internal static bool TargetsMultipleFramework { get; private set; } = true;

    public static void AssignTargetAssembly(Assembly assembly)
    {
        if (VerifierSettings.assembly is not null)
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
        VerifierSettings.assembly = assembly;
    }
}