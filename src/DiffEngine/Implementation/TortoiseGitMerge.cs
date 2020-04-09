using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition TortoiseGitMerge() =>
        new ToolDefinition(
            name: DiffTool.TortoiseGitMerge,
            url: "https://tortoisegit.org/docs/tortoisegitmerge/",
            supportsAutoRefresh: false,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            buildArguments: (tempFile, targetFile) => $"\"{tempFile}\" \"{targetFile}\"",
            windowsExePaths: new[]
            {
                @"%ProgramFiles%\TortoiseGit\bin\TortoiseGitMerge.exe"
            },
            binaryExtensions: Array.Empty<string>(),
            linuxExePaths: Array.Empty<string>(),
            osxExePaths: Array.Empty<string>());
}