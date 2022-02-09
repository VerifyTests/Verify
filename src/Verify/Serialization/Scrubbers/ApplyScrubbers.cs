using System.Runtime.InteropServices;

static class ApplyScrubbers
{
    static char dirSeparator = Path.DirectorySeparatorChar;
    static char altDirSeparator = Path.AltDirectorySeparatorChar;
    static List<KeyValuePair<string, string>> replacements = null!;

    static string ReplaceAltDirChar(this string directory)
    {
        return directory.Replace(dirSeparator, altDirSeparator);
    }

    public static void UseAssembly(string? solutionDir, string projectDir)
    {
        Dictionary<string, string> replacements = new();
        var baseDir = CleanPath(AppDomain.CurrentDomain.BaseDirectory!);
        var altBaseDir = baseDir.ReplaceAltDirChar();
        replacements[baseDir + dirSeparator] = "{CurrentDirectory}";
        replacements[baseDir] = "{CurrentDirectory}";
        replacements[altBaseDir + altDirSeparator] = "{CurrentDirectory}";
        replacements[altBaseDir] = "{CurrentDirectory}";

        var currentDir = CleanPath(Environment.CurrentDirectory);
        var altCurrentDir = currentDir.ReplaceAltDirChar();
        replacements[currentDir + dirSeparator] = "{CurrentDirectory}";
        replacements[currentDir] = "{CurrentDirectory}";
        replacements[altCurrentDir + altDirSeparator] = "{CurrentDirectory}";
        replacements[altCurrentDir] = "{CurrentDirectory}";
#if !NET5_0_OR_GREATER
        if (CodeBaseLocation.CurrentDirectory is not null)
        {
            var codeBaseLocation = CleanPath(CodeBaseLocation.CurrentDirectory);
            var altCodeBaseLocation = codeBaseLocation.ReplaceAltDirChar();
            replacements[codeBaseLocation + dirSeparator] = "{CurrentDirectory}";
            replacements[codeBaseLocation] = "{CurrentDirectory}";
            replacements[altCodeBaseLocation + altDirSeparator] = "{CurrentDirectory}";
            replacements[altCodeBaseLocation] = "{CurrentDirectory}";
        }
#endif

        var tempPath = CleanPath(Path.GetTempPath());
        var altTempPath = tempPath.ReplaceAltDirChar();
        replacements[tempPath + dirSeparator] = "{TempPath}";
        replacements[tempPath] = "{TempPath}";
        replacements[altTempPath + altDirSeparator] = "{TempPath}";
        replacements[altTempPath] = "{TempPath}";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var profileDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var altProfileDir = profileDir.ReplaceAltDirChar();
            replacements[profileDir] = "{UserProfile}";
            replacements[altProfileDir] = "{UserProfile}";
        }

        AddProjectAndSolutionReplacements(solutionDir, projectDir, replacements);
        ApplyScrubbers.replacements = replacements.OrderByDescending(x => x.Key).ToList();
    }

    static void AddProjectAndSolutionReplacements(string? solutionDir, string projectDir, Dictionary<string, string> replacements)
    {
        if (!VerifierSettings.scrubProjectDir &&
            !VerifierSettings.scrubSolutionDir)
        {
            return;
        }
        var altProjectDir = projectDir.ReplaceAltDirChar();
        var altProjectDirTrimmed = altProjectDir.TrimEnd('/', '\\');
        var projectDirTrimmed = projectDir.TrimEnd('/', '\\');

        if (!VerifierSettings.scrubSolutionDir ||
            solutionDir == null)
        {
            replacements[projectDir] = "{ProjectDirectory}";
            replacements[projectDirTrimmed] = "{ProjectDirectory}";
            replacements[altProjectDir] = "{ProjectDirectory}";
            replacements[altProjectDirTrimmed] = "{ProjectDirectory}";

            return;
        }

        var altSolutionDir = solutionDir.ReplaceAltDirChar();
        var altSolutionDirTrimmed = altSolutionDir.TrimEnd('/', '\\');
        var solutionDirectoryTrimmed = solutionDir.TrimEnd('/', '\\');

        replacements[projectDir] = "{ProjectDirectory}";
        replacements[projectDirTrimmed] = "{ProjectDirectory}";
        replacements[altProjectDir] = "{ProjectDirectory}";
        replacements[altProjectDirTrimmed] = "{ProjectDirectory}";

        replacements[solutionDir] = "{SolutionDirectory}";
        replacements[solutionDirectoryTrimmed] = "{SolutionDirectory}";
        replacements[altSolutionDir] = "{SolutionDirectory}";
        replacements[altSolutionDirTrimmed] = "{SolutionDirectory}";
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

        foreach (var replace in replacements)
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