using System;

static partial class DiffTools
{
    static DiffTool Meld() => new DiffTool(
        name: "Meld",
        url: "https://meldmerge.org/",
        shouldTerminate: true,
        buildArguments: pair => $"\"{pair.Received}\" \"{pair.Verified}\"",
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