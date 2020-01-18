using System;

static partial class DiffTools
{
    public static DiffTool SublimeMerge() => new DiffTool(
        name: "Sublime Merge",
        url: "https://www.sublimemerge.com/",
        shouldTerminate: true,
        buildArguments: pair => $"mergetool \"{pair.Received}\" \"{pair.Verified}\"",
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