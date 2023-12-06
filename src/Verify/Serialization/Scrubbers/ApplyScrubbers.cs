// ReSharper disable RedundantSuppressNullableWarningExpression

static class ApplyScrubbers
{
    static char dirSeparator = Path.DirectorySeparatorChar;
    static char altDirSeparator = Path.AltDirectorySeparatorChar;
    static List<KeyValuePair<string, string>> replacements = [];

    static string ReplaceAltDirChar(this string directory) =>
        directory.Replace(dirSeparator, altDirSeparator);

    public static void UseAssembly(string? solutionDir, string projectDir)
    {
        var replacements = new Dictionary<string, string>();
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
            if (!string.IsNullOrWhiteSpace(profileDir))
            {
                var altProfileDir = profileDir.ReplaceAltDirChar();
                replacements[profileDir] = "{UserProfile}";
                replacements[altProfileDir] = "{UserProfile}";
            }
        }

        AddProjectAndSolutionReplacements(solutionDir, projectDir, replacements);
        ApplyScrubbers.replacements = replacements
            .OrderByDescending(_ => _.Key)
            .ToList();
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
            solutionDir is null)
        {
            replacements[projectDir] = "{ProjectDirectory}";
            replacements[projectDirTrimmed] = "{ProjectDirectory}";
            replacements[altProjectDir] = "{ProjectDirectory}";
            replacements[altProjectDirTrimmed] = "{ProjectDirectory}";

            return;
        }

        var altSolutionDir = solutionDir.ReplaceAltDirChar();

        replacements[projectDir] = "{ProjectDirectory}";
        replacements[projectDirTrimmed] = "{ProjectDirectory}";
        replacements[altProjectDir] = "{ProjectDirectory}";
        replacements[altProjectDirTrimmed] = "{ProjectDirectory}";

        replacements[solutionDir] = "{SolutionDirectory}";
        if (solutionDir.Length > 1)
        {
            var solutionDirectoryTrimmed = solutionDir.TrimEnd('/', '\\');
            replacements[solutionDirectoryTrimmed] = "{SolutionDirectory}";
        }

        replacements[altSolutionDir] = "{SolutionDirectory}";
        if (solutionDir.Length > 1)
        {
            var altSolutionDirTrimmed = altSolutionDir.TrimEnd('/', '\\');
            replacements[altSolutionDirTrimmed] = "{SolutionDirectory}";
        }
    }

    public static void ApplyForExtension(string extension, StringBuilder target, VerifySettings settings, Counter counter)
    {
        foreach (var scrubber in settings.InstanceScrubbers)
        {
            scrubber(target, counter);
        }

        if (settings.ExtensionMappedInstanceScrubbers.TryGetValue(extension, out var extensionBasedInstanceScrubbers))
        {
            foreach (var scrubber in extensionBasedInstanceScrubbers)
            {
                scrubber(target, counter);
            }
        }

        if (VerifierSettings.ExtensionMappedGlobalScrubbers.TryGetValue(extension, out var extensionBasedScrubbers))
        {
            foreach (var scrubber in extensionBasedScrubbers)
            {
                scrubber(target, counter);
            }
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(target, counter);
        }

        foreach (var replace in replacements)
        {
            target.ReplaceIfLonger(replace.Key, replace.Value);
        }

        target.FixNewlines();
    }

    public static string ApplyForPropertyValue(string value, VerifySettings settings, Counter counter)
    {
        var builder = new StringBuilder(value);
        foreach (var scrubber in settings.InstanceScrubbers)
        {
            scrubber(builder, counter);
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(builder, counter);
        }

        foreach (var replace in replacements)
        {
            builder.ReplaceIfLonger(replace.Key, replace.Value);
        }

        builder.FixNewlines();
        return builder.ToString();
    }

    static string CleanPath(string directory) =>
        directory.TrimEnd('/', '\\');
}