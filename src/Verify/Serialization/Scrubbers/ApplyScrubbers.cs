using System.Runtime.InteropServices;
using VerifyTests;

static class ApplyScrubbers
{
    static Dictionary<string,string> replacements = new();

    static ApplyScrubbers()
    {
        var baseDirectory = CleanPath(AppDomain.CurrentDomain.BaseDirectory!);
        var altBaseDirectory = baseDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        replacements.Add(baseDirectory + Path.DirectorySeparatorChar, "{CurrentDirectory}");
        replacements.Add(baseDirectory, "{CurrentDirectory}");
        if (baseDirectory != altBaseDirectory)
        {
            replacements.Add(altBaseDirectory + Path.AltDirectorySeparatorChar, "{CurrentDirectory}");
            replacements.Add(altBaseDirectory, "{CurrentDirectory}");
        }

        var currentDirectory = CleanPath(Environment.CurrentDirectory);
        if (baseDirectory != currentDirectory)
        {
            var altCurrentDirectory = currentDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            replacements.Add(currentDirectory + Path.DirectorySeparatorChar, "{CurrentDirectory}");
            replacements.Add(currentDirectory, "{CurrentDirectory}");
            if (currentDirectory != altCurrentDirectory)
            {
                replacements.Add(altCurrentDirectory + Path.AltDirectorySeparatorChar, "{CurrentDirectory}");
                replacements.Add(altCurrentDirectory, "{CurrentDirectory}");
            }
        }
#if !NET5_0_OR_GREATER
        if (CodeBaseLocation.CurrentDirectory is not null)
        {
            var codeBaseLocation = CleanPath(CodeBaseLocation.CurrentDirectory);
            if (codeBaseLocation != currentDirectory && codeBaseLocation != baseDirectory)
            {
                var altCodeBaseLocation = codeBaseLocation.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                replacements.Add(codeBaseLocation + Path.DirectorySeparatorChar, "{CurrentDirectory}");
                replacements.Add(codeBaseLocation, "{CurrentDirectory}");
                if (codeBaseLocation != altCodeBaseLocation)
                {
                    replacements.Add(altCodeBaseLocation + Path.AltDirectorySeparatorChar, "{CurrentDirectory}");
                    replacements.Add(altCodeBaseLocation, "{CurrentDirectory}");
                }
            }
        }
#endif

        var tempPath = CleanPath(Path.GetTempPath());
        var altTempPath = tempPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        replacements.Add(tempPath + Path.DirectorySeparatorChar, "{TempPath}");
        replacements.Add(tempPath, "{TempPath}");
        if (tempPath != altTempPath)
        {
            replacements.Add(altTempPath + Path.AltDirectorySeparatorChar, "{TempPath}");
            replacements.Add(altTempPath, "{TempPath}");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var profileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var altProfileDirectory = profileDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            replacements[profileDirectory] = "{UserProfile}";
            if (profileDirectory != altProfileDirectory)
            {
                replacements[altProfileDirectory] = "{UserProfile}";
            }
        }
    }

    public static void UseAssembly(string? solutionDirectory, string projectDirectory)
    {
        if (!VerifierSettings.scrubProjectDirectory &&
            !VerifierSettings.scrubSolutionDirectory)
        {
            return;
        }

        var altProjectDirectory = projectDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var altProjectDirectoryTrimmed = altProjectDirectory.TrimEnd('/', '\\');
        var projectDirectoryTrimmed = projectDirectory.TrimEnd('/', '\\');

        if (!VerifierSettings.scrubSolutionDirectory ||
            solutionDirectory == null)
        {
            replacements.Add(projectDirectory, "{ProjectDirectory}");
            replacements.Add(projectDirectoryTrimmed, "{ProjectDirectory}");
            if (projectDirectory != altProjectDirectory)
            {
                replacements.Add(altProjectDirectory, "{ProjectDirectory}");
                replacements.Add(altProjectDirectoryTrimmed, "{ProjectDirectory}");
            }

            return;
        }

        var altSolutionDirectory = solutionDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var altSolutionDirectoryTrimmed = altSolutionDirectory.TrimEnd('/', '\\');
        var solutionDirectoryTrimmed = solutionDirectory.TrimEnd('/', '\\');

        replacements[projectDirectory] = "{ProjectDirectory}";
        replacements[projectDirectoryTrimmed] = "{ProjectDirectory}";
        if (projectDirectory != altProjectDirectory)
        {
            replacements[altProjectDirectory] = "{ProjectDirectory}";
            replacements[altProjectDirectoryTrimmed] = "{ProjectDirectory}";
        }

        replacements[solutionDirectory] = "{SolutionDirectory}";
        replacements[solutionDirectoryTrimmed] = "{SolutionDirectory}";
        if (solutionDirectory != altSolutionDirectory)
        {
            replacements[altSolutionDirectory] = "{SolutionDirectory}";
            replacements[altSolutionDirectoryTrimmed] = "{SolutionDirectory}";
        }
    }

    public static void Apply(string extension, StringBuilder target, VerifySettings settings)
    {
        foreach (var scrubber in settings.InstanceScrubbers)
        {
            scrubber(target);
        }

        if (settings.extensionMappedInstanceScrubbers.TryGetValue(extension, out var extensionBasedInstanceScrubbers))
        {
            foreach (var scrubber in extensionBasedInstanceScrubbers)
            {
                scrubber(target);
            }
        }

        if (VerifierSettings.ExtensionMappedGlobalScrubbers.TryGetValue(extension, out var extensionBasedScrubbers))
        {
            foreach (var scrubber in extensionBasedScrubbers)
            {
                scrubber(target);
            }
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(target);
        }

        if (VerifierSettings.ExtensionMappedGlobalScrubbers.TryGetValue(extension, out extensionBasedScrubbers))
        {
            foreach (var scrubber in extensionBasedScrubbers)
            {
                scrubber(target);
            }
        }

        var keyValuePairs = replacements.OrderByDescending(x => x.Key.Length).ToList();
        foreach (var replace in keyValuePairs)
        {
            target.Replace(replace.Key, replace.Value);
        }

        target.FixNewlines();
    }

    static string CleanPath(string directory)
    {
        return directory.TrimEnd('/', '\\');
    }
}