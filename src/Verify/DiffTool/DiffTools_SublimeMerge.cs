using System;

static partial class DiffTools
{
    public static DiffTool SublimeMerge() => new DiffTool(
        name: "Sublime Merge",
        url: "https://www.sublimemerge.com/",
        argumentPrefix: "mergetool ",
        exePaths: new[]
        {
            @"%ProgramFiles%\Sublime Merge\smerge.exe"
        },
        binaryExtensions: Array.Empty<string>());
}