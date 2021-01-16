﻿using System;
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

    public static void Apply(StringBuilder target, List<Action<StringBuilder>> scrubbers)
    {
        foreach (var replace in currentDirectoryReplacements)
        {
            target.Replace(replace, "{CurrentDirectory}");
        }

        target.Replace(tempPath, "{TempPath}");
        target.Replace(altTempPath, "{TempPath}");

        foreach (var scrubber in scrubbers)
        {
            scrubber(target);
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(target);
        }

        target.FixNewlines();
    }

    static string CleanPath(string directory)
    {
        return directory.TrimEnd('/', '\\');
    }
}