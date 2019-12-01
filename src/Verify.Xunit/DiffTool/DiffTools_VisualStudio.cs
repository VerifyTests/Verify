using System;

static partial class DiffTools
{
    public static DiffTool VisualStudio() => new DiffTool(
        name: "VisualStudio",
        url: "https://docs.microsoft.com/en-us/visualstudio/ide/reference/diff",
        argumentPrefix: "/diff ",
        exePaths: new[]
        {
            @"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe",
            @"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.exe",
        },
        binaryExtensions: Array.Empty<string>());
}