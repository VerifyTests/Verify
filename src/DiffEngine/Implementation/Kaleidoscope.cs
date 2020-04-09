using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition Kaleidoscope() =>
        new ToolDefinition(
            name: DiffTool.Kaleidoscope,
            url: "https://www.kaleidoscopeapp.com/",
            supportsAutoRefresh: false,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            buildArguments: (tempFile, targetFile) => $"\"{tempFile}\" \"{targetFile}\"",
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