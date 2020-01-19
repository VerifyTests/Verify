using System;

static partial class DiffTools
{
    static DiffTool CodeCompare() => new DiffTool(
        name: "CodeCompare",
        url: "https://www.devart.com/codecompare/docs/index.html?comparing_via_command_line.htm",
        shouldTerminate: false,
        supportsAutoRefresh: false,
        isMdi: true,
        buildArguments: pair => $"\"{pair.Received}\" \"{pair.Verified}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Devart\Code Compare\CodeCompare.exe"
        },
        osxExePaths: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        binaryExtensions: Array.Empty<string>());
}