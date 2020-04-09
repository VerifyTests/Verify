using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition SublimeMerge() =>
        new ToolDefinition(
            name: DiffTool.SublimeMerge,
            url: "https://www.sublimemerge.com/",
            supportsAutoRefresh: false,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            buildArguments: (tempFile, targetFile) => $"mergetool \"{tempFile}\" \"{targetFile}\"",
            windowsExePaths: new[]
            {
                @"%ProgramFiles%\Sublime Merge\smerge.exe"
            },
            binaryExtensions: Array.Empty<string>(),
            linuxExePaths: new[]
            {
                @"/usr/bin/smerge"
            },
            osxExePaths: new[]
            {
                @"/Applications/smerge.app/Contents/MacOS/smerge"
            });
}