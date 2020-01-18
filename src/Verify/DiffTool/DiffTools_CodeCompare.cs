using System;

static partial class DiffTools
{
    public static DiffTool CodeCompare() => new DiffTool(
        name: "CodeCompare",
        url: "https://www.devart.com/codecompare/docs/index.html?comparing_via_command_line.htm",
        argumentPrefix: null,
        //TODO: verify
        shouldTerminate: false,
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Devart\Code Compare\CodeCompare.exe"
        },
        osxExePaths: Array.Empty<string>(),
        linuxExePaths: Array.Empty<string>(),
        binaryExtensions: Array.Empty<string>());
}