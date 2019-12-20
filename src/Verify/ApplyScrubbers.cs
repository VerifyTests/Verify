using System;
using System.Collections.Generic;
using Verify;

static class ApplyScrubbers
{
    static HashSet<string> currentDirectoryReplacements = new HashSet<string>();

    static ApplyScrubbers()
    {
        currentDirectoryReplacements.Add(CleanPath(AppDomain.CurrentDomain.BaseDirectory));
        currentDirectoryReplacements.Add(CleanPath(Environment.CurrentDirectory));
        currentDirectoryReplacements.Add(CleanPath(CodeBaseLocation.CurrentDirectory));
    }

    public static string Apply(string target, List<Func<string, string>> scrubbers)
    {
        foreach (var replace in currentDirectoryReplacements)
        {
            target = target.Replace(replace, "CurrentDirectory");
        }

        foreach (var scrubber in scrubbers)
        {
            target = scrubber(target);
        }

        foreach (var scrubber in SharedVerifySettings.GlobalScrubbers)
        {
            target = scrubber(target);
        }

        return target;
    }

    static string CleanPath(string directory)
    {
        return directory.TrimEnd('/', '\\');
    }
}