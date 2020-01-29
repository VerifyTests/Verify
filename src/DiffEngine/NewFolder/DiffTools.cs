using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DiffEngine;

public static partial class DiffTools
{
    public static Dictionary<string, ResolvedDiffTool> ExtensionLookup = new Dictionary<string, ResolvedDiffTool>();
    public static List<ResolvedDiffTool> ResolvedDiffTools = new List<ResolvedDiffTool>();

    public static List<DiffTool> Tools()
    {
        return new List<DiffTool>
        {
            VisualStudio(),
            VsCode(),
            WinMerge(),
            CodeCompare(),
            SublimeMerge(),
            Meld(),
            AraxisMerge(),
            P4Merge(),
            BeyondCompare(),
        };
    }

    static DiffTools()
    {
        foreach (var tool in Tools().Where(x => x.Exists))
        {
            var diffTool = new ResolvedDiffTool(
                tool.Name,
                tool.ExePath!,
                tool.BuildArguments,
                tool.IsMdi,
                tool.SupportsAutoRefresh);
            ResolvedDiffTools.Add(diffTool);
            foreach (var ext in tool.BinaryExtensions)
            {
                ExtensionLookup[ext] = diffTool;
            }
        }
    }

    public static bool TryFind(string extension, [NotNullWhen(true)] out ResolvedDiffTool? tool)
    {
        if (Extensions.IsTextExtension(extension))
        {
            tool = ResolvedDiffTools.LastOrDefault();
            return tool != null;
        }

        return ExtensionLookup.TryGetValue(extension, out tool);
    }
}