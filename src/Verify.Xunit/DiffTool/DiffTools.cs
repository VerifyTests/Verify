using System.Collections.Generic;
using System.Linq;

static partial class DiffTools
{
    internal static Dictionary<string, ResolvedDiffTool> ExtensionLookup = new Dictionary<string, ResolvedDiffTool>();
    internal static List<ResolvedDiffTool> ResolvedDiffTools = new List<ResolvedDiffTool>();

    // ReSharper disable once UnusedParameter.Global
    public static bool TryGetTextDiff(string extension, out ResolvedDiffTool diffTool)
    {
        diffTool = ResolvedDiffTools.LastOrDefault();
        return diffTool != null;
    }

    internal static List<DiffTool> Tools()
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
        if (BuildServerDetector.Detected)
        {
            return;
        }
        foreach (var tool in Tools().Where(x=>x.Exists))
        {
            var diffTool = new ResolvedDiffTool(tool.Name, tool.ExePath!, tool.ArgumentPrefix);
            ResolvedDiffTools.Add(diffTool);
            foreach (var ext in tool.BinaryExtensions)
            {
                ExtensionLookup[ext] = diffTool;
            }
        }
    }

    public static bool TryFindForExtension(string extension, out ResolvedDiffTool diffTool)
    {
        return ExtensionLookup.TryGetValue(extension, out diffTool);
    }
}