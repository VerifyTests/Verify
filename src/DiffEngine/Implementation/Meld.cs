using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition Meld() => new ToolDefinition(
        name: DiffTool.Meld,
        url: "https://meldmerge.org/",
        supportsAutoRefresh: false,
        isMdi: false,
        supportsText: true,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles(x86)%\Meld\meld.exe"
        },
        binaryExtensions: Array.Empty<string>(),
        linuxExePaths: new[]
        {
            @"/usr/bin/meld"
        },
        osxExePaths: new[]
        {
            @"/Applications/meld.app/Contents/MacOS/meld"
        });
}