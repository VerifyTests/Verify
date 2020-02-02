using System;

static partial class Implementation
{
    public static DiffTool CodeCompare() => new DiffTool(
        name: "CodeCompare",
        url: "https://www.devart.com/codecompare/docs/index.html?comparing_via_command_line.htm",
        supportsAutoRefresh: false,
        isMdi: true,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Devart\Code Compare\CodeCompare.exe"
        },
        osxExePaths: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        binaryExtensions: Array.Empty<string>());
}