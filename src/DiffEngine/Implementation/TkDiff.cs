using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition TkDiff() => new ToolDefinition(
        name: DiffTool.TkDiff,
        url: "https://sourceforge.net/projects/tkdiff/",
        supportsAutoRefresh: false,
        isMdi: false,
        supportsText: true,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: Array.Empty<string>(),
        binaryExtensions: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        osxExePaths: new[]
        {
            "/Applications/TkDiff.app/Contents/MacOS/tkdiff"
        });
}