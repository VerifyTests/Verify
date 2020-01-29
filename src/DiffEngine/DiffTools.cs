﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DiffEngine
{
    public static class DiffTools
    {
        public static Dictionary<string, ResolvedDiffTool> ExtensionLookup = new Dictionary<string, ResolvedDiffTool>();
        public static List<ResolvedDiffTool> ResolvedDiffTools = new List<ResolvedDiffTool>();

        internal static List<DiffTool> Tools()
        {
            return new List<DiffTool>
            {
               Implementation.VisualStudio(),
               Implementation.VsCode(),
               Implementation.WinMerge(),
               Implementation.CodeCompare(),
               Implementation.SublimeMerge(),
               Implementation.Meld(),
               Implementation.AraxisMerge(),
               Implementation.P4Merge(),
               Implementation.BeyondCompare()
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
}