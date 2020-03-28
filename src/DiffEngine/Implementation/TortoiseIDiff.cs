using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition TortoiseIDiff() => new ToolDefinition(
        name: DiffTool.TortoiseIDiff,
        url: "https://tortoisesvn.net/TortoiseIDiff.html",
        supportsAutoRefresh: false,
        isMdi: false,
        supportsText: false,
        buildArguments: (path1, path2) => $"/left:\"{path1}\" /right:\"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\TortoiseSVN\bin\TortoiseIDiff.exe"
        },
        binaryExtensions: new[]
        {
            "bmp",
            "gif",
            "ico",
            "jpg",
            "jpeg",
            "png",
            "tif",
            "tiff",
        },
        linuxExePaths: Array.Empty<string>(),
        osxExePaths: Array.Empty<string>());
}