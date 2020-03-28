using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition WinMerge() => new ToolDefinition(
        name: DiffTool.WinMerge,
        url: "https://manual.winmerge.org/en/Command_line.html",
        supportsAutoRefresh: true,
        isMdi: false,
        supportsText: true,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles(x86)%\WinMerge\WinMergeU.exe"
        },
        binaryExtensions: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        osxExePaths: Array.Empty<string>());
}