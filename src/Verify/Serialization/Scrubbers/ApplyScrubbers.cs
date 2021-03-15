using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VerifyTests;

static class ApplyScrubbers
{
    static HashSet<string> currentDirectoryReplacements = new();
    static string tempPath;
    static string altTempPath;

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
#if !NET5_0
        if (CodeBaseLocation.CurrentDirectory != null)
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

    public static void Apply(string extension, StringBuilder target, VerifySettings settings)
    {
        foreach (var replace in currentDirectoryReplacements)
        {
            target.Replace(replace, "{CurrentDirectory}");
        }

        target.Replace(tempPath, "{TempPath}");
        target.Replace(altTempPath, "{TempPath}");

        foreach (var scrubber in settings.instanceScrubbers)
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

        target.FixNewlines();
    }

    static string CleanPath(string directory)
    {
        return directory.TrimEnd('/', '\\');
    }
}