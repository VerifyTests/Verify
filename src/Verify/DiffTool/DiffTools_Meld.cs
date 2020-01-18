using System;

static partial class DiffTools
{
    public static DiffTool Meld() => new DiffTool(
        name: "Meld",
        url: "https://meldmerge.org/",
        argumentPrefix: null,
        shouldTerminate: true,
        windowsExePaths: new[]
        {
            @"%ProgramFiles(x86)%\Meld\meld.exe"
        },
        osxExePaths: new[]
        {
            @"/Applications/meld.app/Contents/MacOS/meld"
        },
        linuxExePaths: new[]
        {
            @"/usr/bin/meld"
        },
        binaryExtensions: Array.Empty<string>());
}