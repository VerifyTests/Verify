using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EmptyFiles;

namespace DiffEngine
{
    public static class DiffTools
    {
        internal static Dictionary<string, ResolvedDiffTool> ExtensionLookup = new Dictionary<string, ResolvedDiffTool>();
        internal static List<ResolvedDiffTool> ResolvedDiffTools = new List<ResolvedDiffTool>();
        internal static List<DiffTool> TextDiffTools = new List<DiffTool>();

        internal static List<ToolDefinition> Tools()
        {
            return new List<ToolDefinition>
            {
                Implementation.Rider(),
                Implementation.VisualStudio(),
                Implementation.VsCode(),
                Implementation.TkDiff(),
                Implementation.KDiff3(),
                Implementation.TortoiseIDiff(),
                Implementation.TortoiseGitMerge(),
                Implementation.TortoiseMerge(),
                Implementation.DiffMerge(),
                Implementation.WinMerge(),
                Implementation.CodeCompare(),
                Implementation.Kaleidoscope(),
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
                    tool.SupportsAutoRefresh,
                    tool.BinaryExtensions);
                if (tool.SupportsText)
                {
                    TextDiffTools.Add(diffTool.Name);
                }
                ResolvedDiffTools.Add(diffTool);
                foreach (var ext in tool.BinaryExtensions)
                {
                    ExtensionLookup[ext] = diffTool;
                }
            }
        }

        internal static IEnumerable<ToolDefinition> ReadMachineTools()
        {
            var allTool = Tools().Where(x => x.Exists).ToList();
            var diffOrder = Environment.GetEnvironmentVariable("Verify.DiffToolOrder");
            if (string.IsNullOrWhiteSpace(diffOrder))
            {
                return allTool;
            }

            return ReadToolsFromEnvironmentVariable(diffOrder, allTool);
        }

        internal static IEnumerable<DiffTool> ParseEnvironmentVariable(string diffOrder)
        {
            foreach (var toolString in diffOrder
                .Split(new[] {',', '|', ' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!Enum.TryParse<DiffTool>(toolString, out var diffTool))
                {
                    throw new Exception($"Unable to parse tool from `Verify.DiffToolOrder` environment variable: {toolString}");
                }

                yield return diffTool;
            }
        }

        internal static IEnumerable<ToolDefinition> ReadToolsFromEnvironmentVariable(string diffOrder, List<ToolDefinition> allTools)
        {
            foreach (var diffTool in ParseEnvironmentVariable(diffOrder))
            {
                var definition = allTools.SingleOrDefault(x => x.Name == diffTool);
                if (definition == null)
                {
                    throw new Exception($"`Verify.DiffToolOrder` is configured to use '{diffTool}' but it is not installed.");
                }

                yield return definition;
                allTools.Remove(definition);
            }

            foreach (var definition in allTools)
            {
                yield return definition;
            }
        }

        internal static bool TryFind(string extension, [NotNullWhen(true)] out ResolvedDiffTool? tool)
        {
            if (Extensions.IsText(extension))
            {
                tool = ResolvedDiffTools.LastOrDefault();
                return tool != null;
            }

            return ExtensionLookup.TryGetValue(extension, out tool);
        }

        internal static bool TryFind(DiffTool tool, string extension, [NotNullWhen(true)] out ResolvedDiffTool? resolvedTool)
        {
            if (Extensions.IsText(extension))
            {
                resolvedTool = ResolvedDiffTools.FirstOrDefault(x=>x.Name==tool);
                return resolvedTool != null;
            }

            resolvedTool = ResolvedDiffTools.SingleOrDefault(x => x.Name == tool);
            if (resolvedTool == null)
            {
                return false;
            }

            if (!resolvedTool.BinaryExtensions.Contains(extension))
            {
                resolvedTool = null;
                return false;
            }

            return true;
        }

        public static bool IsDetectedFor(DiffTool diffTool, string extensionOrPath)
        {
            var extension = Extensions.GetExtension(extensionOrPath);
            if (Extensions.IsText(extension))
            {
                return TextDiffTools.Contains(diffTool);
            }

            var tool = ResolvedDiffTools.SingleOrDefault(_ => _.Name == diffTool);
            if (tool == null)
            {
                return false;
            }

            return tool.BinaryExtensions.Contains(extension);
        }
    }
}