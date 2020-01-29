using System;

public static partial class DiffTools
{
    static DiffTool WinMerge() => new DiffTool(
        name: "WinMerge",
        url: "https://manual.winmerge.org/en/Command_line.html",
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
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