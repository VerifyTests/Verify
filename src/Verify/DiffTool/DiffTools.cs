using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

static partial class DiffTools
{
    internal static Dictionary<string, ResolvedDiffTool> ExtensionLookup = new Dictionary<string, ResolvedDiffTool>();
    internal static List<ResolvedDiffTool> ResolvedDiffTools = new List<ResolvedDiffTool>();

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

        foreach (var tool in Tools().Where(x => x.Exists))
        {
            var diffTool = new ResolvedDiffTool(tool.Name, tool.ExePath!, tool.ShouldTerminate, tool.BuildArguments);
            ResolvedDiffTools.Add(diffTool);
            foreach (var ext in tool.BinaryExtensions)
            {
                ExtensionLookup[ext] = diffTool;
            }
        }
    }

    public static bool TryFind(string extension, [NotNullWhen(true)] out ResolvedDiffTool tool)
    {
        if (Extensions.IsTextExtension(extension))
        {
            tool = ResolvedDiffTools.LastOrDefault();
            return true;
        }

        return ExtensionLookup.TryGetValue(extension, out tool);
    }
}