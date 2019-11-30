using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

static partial class DiffTools
{
    internal static Dictionary<string, ResolvedDiffTool> ExtensionLookup = new Dictionary<string, ResolvedDiffTool>();

    static List<DiffTool> Tools = new List<DiffTool>
    {
        VisualStudio(),
        SublimeMerge(),
        Meld(),
        P4Merge(),
        BeyondCompare(),
    };

    static DiffTools()
    {
        foreach (var tool in Tools)
        {
            if (!TryFindTool(tool, out var exePath))
            {
                continue;
            }

            var diffTool = new ResolvedDiffTool(tool.Name, exePath, tool.ArgumentFormat);
            foreach (var ext in tool.BinaryExtensions)
            {
                ExtensionLookup[ext] = diffTool;
            }
        }
    }

    static bool TryFindTool(DiffTool tool, [NotNullWhen(true)] out string? path)
    {
        foreach (var exePath in tool.ExePaths)
        {
            var expanded = Environment.ExpandEnvironmentVariables(exePath);
            if (!File.Exists(expanded))
            {
                continue;
            }

            path = expanded;
            return true;
        }

        path = null;
        return false;
    }

    public static IEnumerable<DiffTool> FoundTools()
    {
        foreach (var tool in Tools)
        {
            foreach (var exePath in tool.ExePaths)
            {
                var expanded = Environment.ExpandEnvironmentVariables(exePath);
                if (File.Exists(expanded))
                {
                    yield return tool;
                }
            }
        }
    }
}