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
        binaryExtensions: Array.Empty<string>());
}