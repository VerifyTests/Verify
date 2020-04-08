using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition VsCode() => new ToolDefinition(
        name: DiffTool.VisualStudioCode,
        url: "https://code.visualstudio.com",
        supportsAutoRefresh: true,
        isMdi: true,
        supportsText: true,
        buildArguments: (tempFile, targetFile) => $"--diff \"{targetFile}\" \"{tempFile}\"",
        windowsExePaths: new[]
        {
            @"%LOCALAPPDATA%\Programs\Microsoft VS Code\code.exe"
        },
        binaryExtensions: Array.Empty<string>(),
        linuxExePaths: new[]
        {
            @"/usr/local/bin/code"
        },
        osxExePaths: new[]
        {
            "/Applications/Visual Studio Code.app/Contents/Resources/app/bin/code"
        },
        notes:@"
 * [Command line reference](https://code.visualstudio.com/docs/editor/command-line)");
}