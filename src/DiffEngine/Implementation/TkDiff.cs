using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition TkDiff() =>
        new ToolDefinition(
            name: DiffTool.TkDiff,
            url: "https://sourceforge.net/projects/tkdiff/",
            supportsAutoRefresh: false,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            buildArguments: (tempFile, targetFile) => $"\"{tempFile}\" \"{targetFile}\"",
            windowsExePaths: Array.Empty<string>(),
            binaryExtensions: Array.Empty<string>(),
            linuxExePaths: Array.Empty<string>(),
            osxExePaths: new[]
            {
                "/Applications/TkDiff.app/Contents/MacOS/tkdiff"
            });
}