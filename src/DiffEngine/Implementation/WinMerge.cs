using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition WinMerge() =>
        new ToolDefinition(
            name: DiffTool.WinMerge,
            url: "https://winmerge.org/",
            supportsAutoRefresh: true,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            buildArguments: (tempFile, targetFile) => $"\"{tempFile}\" \"{targetFile}\"",
            windowsExePaths: new[]
            {
                @"%ProgramFiles(x86)%\WinMerge\WinMergeU.exe"
            },
            binaryExtensions: Array.Empty<string>(),
            linuxExePaths: Array.Empty<string>(),
            osxExePaths: Array.Empty<string>(),
            notes: @"
 * [Command line reference](https://manual.winmerge.org/en/Command_line.html)");
}