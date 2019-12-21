using System;

static partial class DiffTools
{
    public static DiffTool WinMerge() => new DiffTool(
        name: "WinMerge",
        url: "https://manual.winmerge.org/en/Command_line.html",
        argumentPrefix: string.Empty,
        windowsExePaths: new[]
        {
            @"%ProgramFiles(x86)%\WinMerge\WinMergeU.exe"
        },
        osxExePaths: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        binaryExtensions: Array.Empty<string>());
}