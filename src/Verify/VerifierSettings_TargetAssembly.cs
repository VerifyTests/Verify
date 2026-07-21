#pragma warning disable VerifyTestsProjectDir
namespace VerifyTests;

public static partial class VerifierSettings
{
    static Assembly? assembly;

    [Experimental("VerifyTestsProjectDir")]
    public static string ProjectDir { get; private set; } = null!;

    internal static string? SolutionDir { get; private set; }

    // The obj directory, used to store the received map files. Null when the project does not consume
    // Verify's build props, in which case no maps are written.
    internal static string? IntermediateDir { get; private set; }

    internal static bool TargetsMultipleFramework { get; private set; } = true;

    [Experimental("VerifierSettingsTestAssembly")]
    public static Assembly Assembly
    {
        get
        {
            if (assembly == null)
            {
                throw new InvalidOperationException("Assembly must be set before calling this method.");
            }

            return assembly;
        }
    }

    static Lock locker = new();

    public static void AssignTargetAssembly(Assembly assembly)
    {
        if (VerifierSettings.assembly is not null)
        {
            return;
        }

        using (locker.EnterScope())
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
            AttributeReader.TryGetIntermediateDirectory(assembly, out var intermediateDir);
            IntermediateDir = intermediateDir;
            if (AttributeReader.TryGetTargetFrameworks(assembly, out var targetFrameworks))
            {
                TargetsMultipleFramework = targetFrameworks.Contains(';');
            }

            DirectoryReplacements.UseAssembly(solutionDir, ProjectDir);
            VerifierSettings.assembly = assembly;
        }
    }
}