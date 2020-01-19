using System;

static partial class DiffTools
{
    static DiffTool WinMerge() => new DiffTool(
        name: "WinMerge",
        url: "https://manual.winmerge.org/en/Command_line.html",
        buildArguments: pair => $"\"{pair.Received}\" \"{pair.Verified}\"",
        supportsAutoRefresh: true,
        isMdi: false,
        windowsExePaths: new[]
        {
            @"%ProgramFiles(x86)%\WinMerge\WinMergeU.exe"
        },
        osxExePaths: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        binaryExtensions: Array.Empty<string>());
}