using System;
using DiffEngine;

static partial class Implementation
{
    public static DiffTool VisualStudio() => new DiffTool(
        name: "VisualStudio",
        url: "https://docs.microsoft.com/en-us/visualstudio/ide/reference/diff",
        supportsAutoRefresh: true,
        isMdi: true,
        // Verified before Received since only detects and refresh the diff based on the first file
        buildArguments: (path1, path2) => $"/diff \"{path2}\" \"{path1}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.exe",
            @"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Professional\Common7\IDE\devenv.exe",
            @"%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\devenv.exe",
        },
        osxExePaths: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        binaryExtensions: Array.Empty<string>());
}