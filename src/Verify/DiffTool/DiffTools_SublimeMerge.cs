using System;

static partial class DiffTools
{
    public static DiffTool SublimeMerge() => new DiffTool(
        name: "Sublime Merge",
        url: "https://www.sublimemerge.com/",
        argumentPrefix: "mergetool ",
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