using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VerifyTests;

static class ApplyScrubbers
{
    static HashSet<string> currentDirectoryReplacements = new();
    static string tempPath;

    static ApplyScrubbers()
    {
        currentDirectoryReplacements.Add(CleanPath(AppDomain.CurrentDomain.BaseDirectory));
        currentDirectoryReplacements.Add(CleanPath(Environment.CurrentDirectory));
#if !NET5_0
        if (CodeBaseLocation.CurrentDirectory != null)
        {
            currentDirectoryReplacements.Add(CleanPath(CodeBaseLocation.CurrentDirectory));
        }
#endif
        tempPath = CleanPath(Path.GetTempPath());
    }

    public static void Apply(StringBuilder target, List<Action<StringBuilder>> scrubbers)
    {
        foreach (var replace in currentDirectoryReplacements)
        {
            target.Replace(replace, "CurrentDirectory");
        }

        target.Replace(tempPath, "TempPath");

        foreach (var scrubber in scrubbers)
        {
            scrubber(target);
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(target);
        }

        if (scrubbers.Any() || VerifierSettings.GlobalScrubbers.Any())
        {
            target.FixNewlines();
        }
    }

    static string CleanPath(string directory)
    {
        return directory.TrimEnd('/', '\\');
    }
}