// ReSharper disable RedundantSuppressNullableWarningExpression

static class ApplyScrubbers
{
    static List<KeyValuePair<string, string>> replacements = [];

    static string ReplaceAltDirChar(this string directory) =>
        directory.Replace(IoHelpers.DirectorySeparator, IoHelpers.AltDirectorySeparator);

    public static void UseAssembly(string? solutionDir, string projectDir)
    {
        var replacements = new Dictionary<string, string>();
        var baseDir = CleanPath(AppDomain.CurrentDomain.BaseDirectory!);
        var altBaseDir = baseDir.ReplaceAltDirChar();
        replacements[baseDir + IoHelpers.DirectorySeparator] = "{CurrentDirectory}";
        replacements[baseDir] = "{CurrentDirectory}";
        replacements[altBaseDir + IoHelpers.AltDirectorySeparator] = "{CurrentDirectory}";
        replacements[altBaseDir] = "{CurrentDirectory}";

        var currentDir = CleanPath(Environment.CurrentDirectory);
        var altCurrentDir = currentDir.ReplaceAltDirChar();
        replacements[currentDir + IoHelpers.DirectorySeparator] = "{CurrentDirectory}";
        replacements[currentDir] = "{CurrentDirectory}";
        replacements[altCurrentDir + IoHelpers.AltDirectorySeparator] = "{CurrentDirectory}";
        replacements[altCurrentDir] = "{CurrentDirectory}";
#if !NET6_0_OR_GREATER
        if (CodeBaseLocation.CurrentDirectory is not null)
        {
            var codeBaseLocation = CleanPath(CodeBaseLocation.CurrentDirectory);
            var altCodeBaseLocation = codeBaseLocation.ReplaceAltDirChar();
            replacements[codeBaseLocation + IoHelpers.DirectorySeparator] = "{CurrentDirectory}";
            replacements[codeBaseLocation] = "{CurrentDirectory}";
            replacements[altCodeBaseLocation + IoHelpers.AltDirectorySeparator] = "{CurrentDirectory}";
            replacements[altCodeBaseLocation] = "{CurrentDirectory}";
        }
#endif

        var tempPath = CleanPath(Path.GetTempPath());
        var altTempPath = tempPath.ReplaceAltDirChar();
        replacements[tempPath + IoHelpers.DirectorySeparator] = "{TempPath}";
        replacements[tempPath] = "{TempPath}";
        replacements[altTempPath + IoHelpers.AltDirectorySeparator] = "{TempPath}";
        replacements[altTempPath] = "{TempPath}";

        if (VerifierSettings.scrubUserProfile)
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
            .OrderByDescending(_ => _.Key.Length)
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
            scrubber(target, counter, settings);
        }

        foreach (var replace in replacements)
        {
            target.ReplaceIfLonger(replace.Key, replace.Value);
        }

        target.FixNewlines();
    }

    public static CharSpan ApplyForPropertyValue(CharSpan value, VerifySettings settings, Counter counter)
    {
        var builder = new StringBuilder(value.Length);
        builder.Append(value);
        ApplyForPropertyValue(settings, counter, builder);
        return builder.AsSpan();
    }

    public static void ApplyForPropertyValue(VerifySettings settings, Counter counter, StringBuilder builder)
    {
        foreach (var scrubber in settings.InstanceScrubbers)
        {
            scrubber(builder, counter);
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(builder, counter, settings);
        }

        foreach (var replace in replacements)
        {
            builder.ReplaceIfLonger(replace.Key, replace.Value);
        }

        builder.FixNewlines();
    }

    static string CleanPath(string directory) =>
        directory.TrimEnd('/', '\\');
}