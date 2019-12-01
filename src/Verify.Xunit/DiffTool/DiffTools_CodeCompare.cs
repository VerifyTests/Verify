using System;

static partial class DiffTools
{
    public static DiffTool CodeCompare() => new DiffTool(
        name: "CodeCompare",
        url: "https://www.devart.com/codecompare/docs/index.html?comparing_via_command_line.htm",
        argumentPrefix: string.Empty,
        exePaths: new[]
        {
            @"%ProgramFiles%\Devart\Code Compare\CodeCompare.exe"
        },
        binaryExtensions: Array.Empty<string>());
}