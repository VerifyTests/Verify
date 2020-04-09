using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition CodeCompare() =>
        new ToolDefinition(
            name: DiffTool.CodeCompare,
            url: "https://www.devart.com/codecompare/",
            supportsAutoRefresh: false,
            isMdi: true,
            supportsText: true,
            requiresTarget: true,
            buildArguments: (tempFile, targetFile) => $"\"{tempFile}\" \"{targetFile}\"",
            windowsExePaths: new[]
            {
                @"%ProgramFiles%\Devart\Code Compare\CodeCompare.exe"
            },
            binaryExtensions: Array.Empty<string>(),
            linuxExePaths: Array.Empty<string>(),
            osxExePaths: Array.Empty<string>(),
            notes: @"
 * [Command line reference](https://www.devart.com/codecompare/docs/index.html?comparing_via_command_line.htm)");
}