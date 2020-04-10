using System;
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
        internal static List<ResolvedDiffTool> TextDiffTools = new List<ResolvedDiffTool>();

        public static void AddCustomTool(
            bool supportsAutoRefresh,
            bool isMdi,
            bool supportsText,
            bool requiresTarget,
            Func<string, string, string> buildArguments,
            string exePath,
            string[] binaryExtensions)
        {
            Guard.AgainstNull(binaryExtensions, nameof(binaryExtensions));
            Guard.AgainstNull(buildArguments, nameof(buildArguments));
            Guard.FileExists(exePath, nameof(exePath));
            var tool = new ResolvedDiffTool(null, exePath, buildArguments, isMdi, supportsAutoRefresh, binaryExtensions, requiresTarget);
            if (supportsText)
            {
                TextDiffTools.Insert(0, tool);
            }

            ResolvedDiffTools.Insert(0, tool);
            foreach (var extension in binaryExtensions)
            {
                var cleanedExtension = Extensions.GetExtension(extension);
                ExtensionLookup[cleanedExtension] = tool;
            }
        }

        internal static List<ToolDefinition> Tools()
        {
            return new List<ToolDefinition>
            {
                Implementation.BeyondCompare(),
                Implementation.P4Merge(),
                Implementation.AraxisMerge(),
                Implementation.Meld(),
                Implementation.SublimeMerge(),
                Implementation.Kaleidoscope(),
                Implementation.CodeCompare(),
                Implementation.WinMerge(),
                Implementation.DiffMerge(),
                Implementation.TortoiseMerge(),
                Implementation.TortoiseGitMerge(),
                Implementation.TortoiseIDiff(),
                Implementation.KDiff3(),
                Implementation.TkDiff(),
                Implementation.VsCode(),
                Implementation.VisualStudio(),
                Implementation.Rider()
            };
        }

        static DiffTools()
        {
            var diffOrder = Environment.GetEnvironmentVariable("Verify.DiffToolOrder");
            IEnumerable<DiffTool> order;
            bool throwForNoTool;
            if (string.IsNullOrWhiteSpace(diffOrder))
            {
                throwForNoTool = false;
                order = Enum.GetValues(typeof(DiffTool)).Cast<DiffTool>();
            }
            else
            {
                throwForNoTool = true;
                order = ParseEnvironmentVariable(diffOrder);
            }

            var tools = ToolsByOrder(throwForNoTool, order);

            InitLookups(tools);
        }

        public static void UseOrder(params DiffTool[] order)
        {
            UseOrder(false, order);
        }

        public static void UseOrder(bool throwForNoTool, params DiffTool[] order)
        {
            Guard.AgainstNullOrEmpty(order, nameof(order));
            var tools = ToolsByOrder(throwForNoTool, order);
            ExtensionLookup.Clear();
            ResolvedDiffTools.Clear();
            TextDiffTools.Clear();
            InitLookups(tools);
        }

        static void InitLookups(IEnumerable<ToolDefinition> tools)
        {
            foreach (var tool in tools)
            {
                var diffTool = new ResolvedDiffTool(
                    tool.Name,
                    tool.ExePath!,
                    tool.BuildArguments,
                    tool.IsMdi,
                    tool.SupportsAutoRefresh,
                    tool.BinaryExtensions,
                    tool.RequiresTarget);
                if (tool.SupportsText)
                {
                    TextDiffTools.Add(diffTool);
                }

                ResolvedDiffTools.Add(diffTool);
                foreach (var ext in tool.BinaryExtensions)
                {
                    if (!ExtensionLookup.ContainsKey(ext))
                    {
                        ExtensionLookup[ext] = diffTool;
                    }
                }
            }
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

        static IEnumerable<ToolDefinition> ToolsByOrder(bool throwForNoTool, IEnumerable<DiffTool> order)
        {
            var allTools = Tools().Where(x => x.Exists).ToList();
            foreach (var diffTool in order)
            {
                var definition = allTools.SingleOrDefault(x => x.Name == diffTool);
                if (definition == null)
                {
                    if (!throwForNoTool)
                    {
                        continue;
                    }

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

        internal static bool TryFind(
            string extension,
            [NotNullWhen(true)] out ResolvedDiffTool? tool)
        {
            if (Extensions.IsText(extension))
            {
                tool = TextDiffTools.FirstOrDefault();
                return tool != null;
            }

            return ExtensionLookup.TryGetValue(extension, out tool);
        }

        internal static bool TryFind(
            DiffTool tool,
            string extension,
            [NotNullWhen(true)] out ResolvedDiffTool? resolvedTool)
        {
            if (Extensions.IsText(extension))
            {
                resolvedTool = TextDiffTools.FirstOrDefault(x => x.Tool == tool);
                return resolvedTool != null;
            }

            resolvedTool = ResolvedDiffTools.SingleOrDefault(x => x.Tool == tool);
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
                return TextDiffTools.Any(x => x.Tool == diffTool);
            }

            var tool = ResolvedDiffTools.SingleOrDefault(_ => _.Tool == diffTool);
            if (tool == null)
            {
                return false;
            }

            return tool.BinaryExtensions.Contains(extension);
        }
    }
}