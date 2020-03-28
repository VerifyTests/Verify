using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition Kaleidoscope() => new ToolDefinition(
        name: DiffTool.Kaleidoscope,
        url: "https://www.kaleidoscopeapp.com/",
        supportsAutoRefresh: false,
        isMdi: false,
        supportsText: true,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: Array.Empty<string>(),
        binaryExtensions: new[]
        {
            "bmp",
            "gif",
            "ico",
            "jpg",
            "jpeg",
            "png",
            "tiff",
            "tif",
        },
        linuxExePaths: Array.Empty<string>(),
        osxExePaths: new[]
        {
            "/usr/local/bin/ksdiff"
        });
}