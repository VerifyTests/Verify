using System;

public static partial class DiffTools
{
    static DiffTool SublimeMerge() => new DiffTool(
        name: "Sublime Merge",
        url: "https://www.sublimemerge.com/",
        supportsAutoRefresh: false,
        isMdi: false,
        buildArguments: (path1, path2) => $"mergetool \"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Sublime Merge\smerge.exe"
        },
        osxExePaths: new[]
        {
            @"/Applications/smerge.app/Contents/MacOS/smerge"
        },
        linuxExePaths: new[]
        {
            @"/usr/bin/smerge"
        },
        binaryExtensions: Array.Empty<string>());
}