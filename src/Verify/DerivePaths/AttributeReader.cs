using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VerifyTests
{
    public static class AttributeReader
    {
        static ConcurrentDictionary<Assembly, (string projectDirectory, Action<StringBuilder> replacements)> cache = new();

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

        internal static (string projectDirectory, Action<StringBuilder> replacements) GetAssemblyInfo(Assembly assembly)
        {
            return cache.GetOrAdd(assembly, _ =>
            {
                var projectDirectory = GetProjectDirectory(_);
                return (projectDirectory, GetReplacements(_, projectDirectory));
            });
        }

        static Action<StringBuilder> GetReplacements(Assembly assembly, string projectDirectory)
        {
            if (!VerifierSettings.scrubProjectDirectory &&
                !VerifierSettings.scrubSolutionDirectory)
            {
                return _ => { };
            }

            var altProjectDirectory = projectDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var altProjectDirectoryTrimmed = altProjectDirectory.TrimEnd('/', '\\');
            var projectDirectoryTrimmed = projectDirectory.TrimEnd('/', '\\');

            if (!VerifierSettings.scrubSolutionDirectory ||
                !TryGetSolutionDirectory(assembly, out var solutionDirectory))
            {
                return builder =>
                {
                    builder.Replace(projectDirectory, "{ProjectDirectory}");
                    builder.Replace(projectDirectoryTrimmed, "{ProjectDirectory}");
                    builder.Replace(altProjectDirectory, "{ProjectDirectory}");
                    builder.Replace(altProjectDirectoryTrimmed, "{ProjectDirectory}");
                };
            }

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
    }
}