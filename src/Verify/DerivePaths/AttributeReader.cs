using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

static class AttributeReader
{
    private static ConcurrentDictionary<Assembly, (string projectDirectory, Action<StringBuilder> replacements)> cache = new();

    static string GetProjectDirectory(Assembly assembly)
    {
        var projectDirectory = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(x => x.Key == "Verify.ProjectDirectory")
            ?.Value;
        if (projectDirectory != null)
        {
            return projectDirectory;
        }

        throw new("Could not find a `AssemblyMetadataAttribute` named `Verify.ProjectDirectory`.");
    }

    static bool TryGetSolutionDirectory(Assembly assembly, out string? solutionDirectory)
    {
        solutionDirectory = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(x => x.Key == "Verify.SolutionDirectory")
            ?.Value;
        return solutionDirectory != null;
    }

    public static (string projectDirectory, Action<StringBuilder> replacements) GetAssemblyInfo(Assembly assembly)
    {
        return cache.GetOrAdd(assembly, _ =>
        {
            var projectDirectory = GetProjectDirectory(_);
            return (projectDirectory, GetReplacements(_, projectDirectory));
        });
    }

    static Action<StringBuilder> GetReplacements(Assembly assembly, string projectDirectory)
    {
        var altProjectDirectory = projectDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var altProjectDirectoryTrimmed = altProjectDirectory.TrimEnd('/', '\\');
        var projectDirectoryTrimmed = projectDirectory.TrimEnd('/', '\\');

        if (TryGetSolutionDirectory(assembly, out var solutionDirectory))
        {
            var altSolutionDirectory = solutionDirectory!.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var altSolutionDirectoryTrimmed = altSolutionDirectory.TrimEnd('/', '\\');
            var solutionDirectoryTrimmed = solutionDirectory.TrimEnd('/', '\\');
            return builder =>
            {
                builder.Replace(projectDirectory, "{ProjectDirectory}");
                builder.Replace(projectDirectoryTrimmed, "{ProjectDirectory}");
                builder.Replace(altProjectDirectory, "{ProjectDirectory}");
                builder.Replace(altProjectDirectoryTrimmed, "{ProjectDirectory}");

                builder.Replace(solutionDirectory, "{SolutionDirectory}");
                builder.Replace(solutionDirectoryTrimmed, "{SolutionDirectory}");
                builder.Replace(altSolutionDirectory, "{SolutionDirectory}");
                builder.Replace(altSolutionDirectoryTrimmed, "{SolutionDirectory}");
            };
        }

        return builder =>
        {
            builder.Replace(projectDirectory, "{ProjectDirectory}");
            builder.Replace(projectDirectoryTrimmed, "{ProjectDirectory}");
            builder.Replace(altProjectDirectory, "{ProjectDirectory}");
            builder.Replace(altProjectDirectoryTrimmed, "{ProjectDirectory}");
        };
    }
}