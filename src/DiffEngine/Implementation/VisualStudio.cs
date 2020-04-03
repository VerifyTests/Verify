using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition VisualStudio() => new ToolDefinition(
        name: DiffTool.VisualStudio,
        url: "https://docs.microsoft.com/en-us/visualstudio/ide/reference/diff",
        supportsAutoRefresh: true,
        isMdi: true,
        supportsText: true,
        buildArguments: (tempFile, targetFile) => $"/diff \"{targetFile}\" \"{tempFile}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe",
            @"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.exe",
            @"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe",
        },
        binaryExtensions: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        osxExePaths: Array.Empty<string>());
}