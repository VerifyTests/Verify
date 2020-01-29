using System;

static partial class Implementation
{
    public static DiffTool Meld() => new DiffTool(
        name: "Meld",
        url: "https://meldmerge.org/",
        supportsAutoRefresh: false,
        isMdi: false,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
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