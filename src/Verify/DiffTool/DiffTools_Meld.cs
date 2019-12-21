using System;

static partial class DiffTools
{
    public static DiffTool Meld() => new DiffTool(
        name: "Meld",
        url: "https://meldmerge.org/",
        argumentPrefix: string.Empty,
        windowsExePaths: new[]
        {
            @"%ProgramFiles(x86)%\Meld\meld.exe"
        },
        binaryExtensions: Array.Empty<string>());
}