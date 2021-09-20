using VerifyTests;

static class ApplyScrubbers
{
    static HashSet<string> currentDirectoryReplacements = new();
    static string tempPath;
    static string altTempPath;
    static Action<StringBuilder> sharedReplacements = null!;

    static ApplyScrubbers()
    {
        var baseDirectory = CleanPath(AppDomain.CurrentDomain.BaseDirectory);
        var altBaseDirectory = baseDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        currentDirectoryReplacements.Add(baseDirectory);
        currentDirectoryReplacements.Add(altBaseDirectory);
        var currentDirectory = CleanPath(Environment.CurrentDirectory);
        var altCurrentDirectory = currentDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        currentDirectoryReplacements.Add(currentDirectory);
        currentDirectoryReplacements.Add(altCurrentDirectory);
#if !NET5_0_OR_GREATER
        if (CodeBaseLocation.CurrentDirectory is not null)
        {
            var codeBaseLocation = CleanPath(CodeBaseLocation.CurrentDirectory);
            var altCodeBaseLocation = codeBaseLocation.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            currentDirectoryReplacements.Add(codeBaseLocation);
            currentDirectoryReplacements.Add(altCodeBaseLocation);
        }
#endif
        tempPath = CleanPath(Path.GetTempPath());
        altTempPath = tempPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    

    public static void UseAssembly(string? solutionDirectory, string projectDirectory)
    {
        sharedReplacements = GetReplacements(solutionDirectory, projectDirectory);
    }

     static Action<StringBuilder> GetReplacements(string? solutionDirectory, string projectDirectory)
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
                (solutionDirectory == null))
            {
                return builder =>
                {
                    builder.Replace(projectDirectory, "{ProjectDirectory}");
                    builder.Replace(projectDirectoryTrimmed, "{ProjectDirectory}");
                    builder.Replace(altProjectDirectory, "{ProjectDirectory}");
                    builder.Replace(altProjectDirectoryTrimmed, "{ProjectDirectory}");
                };
            }

            var altSolutionDirectory = solutionDirectory.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
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
    public static void Apply(string extension, StringBuilder target, VerifySettings settings)
    {
        foreach (var replace in currentDirectoryReplacements)
        {
            target.Replace(replace, "{CurrentDirectory}");
        }

        target.Replace(tempPath, "{TempPath}");
        target.Replace(altTempPath, "{TempPath}");

        foreach (var scrubber in settings.InstanceScrubbers)
        {
            scrubber(target);
        }

        sharedReplacements(target);

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

        target.FixNewlines();
    }

    static string CleanPath(string directory)
    {
        return directory.TrimEnd('/', '\\');
    }
}