using System.Linq;
using System.Reflection;
using VerifyTests;

static class AttributeReader
{
    public static string GetProjectDirectory(Assembly assembly)
    {
        var projectDirectory = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(x => x.Key == "Verify.ProjectDirectory")
            ?.Value;
        if (projectDirectory != null)
        {
            return projectDirectory;
        }

        throw InnerVerifier.exceptionBuilder("Could not find a `AssemblyMetadataAttribute` named `Verify.ProjectDirectory`.");
    }

    public static bool TryGetSolutionDirectory(Assembly assembly, out string? solutionDirectory)
    {
        solutionDirectory = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(x => x.Key == "Verify.SolutionDirectory")
            ?.Value;
        return solutionDirectory != null;
    }
}