using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition TortoiseMerge() => new ToolDefinition(
        name: DiffTool.TortoiseMerge,
        url: "https://tortoisesvn.net/TortoiseMerge.html",
        supportsAutoRefresh: false,
        isMdi: false,
        supportsText: true,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\TortoiseSVN\bin\TortoiseMerge.exe"
        },
        binaryExtensions: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        osxExePaths: Array.Empty<string>());
}