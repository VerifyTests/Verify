using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition CodeCompare() => new ToolDefinition(
        name: DiffTool.CodeCompare,
        url: "https://www.devart.com/codecompare/docs/index.html?comparing_via_command_line.htm",
        supportsAutoRefresh: false,
        isMdi: true,
        supportsText: true,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Devart\Code Compare\CodeCompare.exe"
        },
        binaryExtensions: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        osxExePaths: Array.Empty<string>());
}