using System;
using System.Collections.Generic;
using Verify;

static class ApplyScrubbers
{
    public static string Apply(string target, List<Func<string, string>> scrubbers)
    {
        var baseDirectory = CleanPath(AppDomain.CurrentDomain.BaseDirectory);
        target = target.Replace(baseDirectory, "CurrentDirectory");
        var currentDirectory = CleanPath(Environment.CurrentDirectory);
        target = target.Replace(currentDirectory, "CurrentDirectory");
        var codeBase = CleanPath(CodeBaseLocation.CurrentDirectory);
        target = target.Replace(codeBase, "CurrentDirectory");

        foreach (var scrubber in scrubbers)
        {
            target = scrubber(target);
        }

        foreach (var scrubber in Global.GlobalScrubbers)
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