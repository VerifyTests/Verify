using System;

static partial class DiffTools
{
    public static DiffTool VsCode() => new DiffTool(
        name: "Visual Studio Code",
        url: "https://code.visualstudio.com/docs/editor/command-line",
        argumentPrefix: "--diff ",
        windowsExePaths: new[]
        {
            @"%LOCALAPPDATA%\Programs\Microsoft VS Code\code.exe"
        },
        linuxExePaths: new[]
        {
            //TODO:
            @"%LOCALAPPDATA%\Programs\Microsoft VS Code\code.exe"
        },
        osxExePaths: new[]
        {
            "/Applications/Visual Studio Code.app/Contents/Resources/app/bin/code"
        },
        binaryExtensions: Array.Empty<string>());
}