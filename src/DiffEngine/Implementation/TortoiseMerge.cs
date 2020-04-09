using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition TortoiseMerge() =>
        new ToolDefinition(
            name: DiffTool.TortoiseMerge,
            url: "https://tortoisesvn.net/TortoiseMerge.html",
            supportsAutoRefresh: false,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            buildArguments: (tempFile, targetFile) => $"\"{tempFile}\" \"{targetFile}\"",
            windowsExePaths: new[]
            {
                @"%ProgramFiles%\TortoiseSVN\bin\TortoiseMerge.exe"
            },
            binaryExtensions: Array.Empty<string>(),
            linuxExePaths: Array.Empty<string>(),
            osxExePaths: Array.Empty<string>());
}