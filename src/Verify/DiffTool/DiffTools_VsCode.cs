using System;

static partial class DiffTools
{
    static DiffTool VsCode() => new DiffTool(
        name: "Visual Studio Code",
        url: "https://code.visualstudio.com/docs/editor/command-line",
        shouldTerminate: false,
        supportsAutoRefresh: true,
        isMdi: true,
        // Verified before Received only detects and refresh the diff based on the first file
        buildArguments: pair => $"--diff \"{pair.Verified}\" \"{pair.Received}\"",
        windowsExePaths: new[]
        {
            @"%LOCALAPPDATA%\Programs\Microsoft VS Code\code.exe"
        },
        linuxExePaths: new[]
        {
            @"/usr/local/bin/code"
        },
        osxExePaths: new[]
        {
            "/Applications/Visual Studio Code.app/Contents/Resources/app/bin/code"
        },
        binaryExtensions: Array.Empty<string>());
}