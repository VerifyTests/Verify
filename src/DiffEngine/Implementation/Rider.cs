using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition Rider() => new ToolDefinition(
        name: DiffTool.Rider,
        url: "https://www.jetbrains.com/rider/",
        supportsAutoRefresh: false,
        isMdi: false,
        supportsText: true,
        buildArguments: (tempFile, targetFile) => $" diff \"{tempFile}\" \"{targetFile}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\JetBrains\JetBrains Rider *\bin\rider64.exe"
        },
        binaryExtensions: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        //@"/Applications/Rider*/Contents/MacOS/rider"
        // not current supported on macos since the diff commandline does not work
        osxExePaths: Array.Empty<string>());
}