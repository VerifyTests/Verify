using System.Linq;
using System.Reflection;
using VerifyTests;

static class ProjectDirectoryReader
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
}