using System;

static partial class DiffTools
{
    public static DiffTool Meld() => new DiffTool(
        name: "Meld",
        url: "https://meldmerge.org/",
        argumentFormat: "{receivedPath} {verifiedPath}",
        exePaths: new[]
        {
            @"%ProgramFiles(x86)%\Meld\meld.exe"
        },
        binaryExtensions: Array.Empty<string>());
}