using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition DiffMerge() => new ToolDefinition(
        name: DiffTool.DiffMerge,
        url: "https://www.sourcegear.com/diffmerge/",
        supportsAutoRefresh: false,
        isMdi: false,
        supportsText: true,
        buildArguments: (tempFile, targetFile) => $"--nosplash \"{tempFile}\" \"{targetFile}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\SourceGear\Common\DiffMerge\sgdm.exe"
        },
        binaryExtensions: Array.Empty<string>(),
        linuxExePaths: new[]
        {
            "/usr/bin/diffmerge"
        },
        osxExePaths: new[]
        {
            "/Applications/DiffMerge.app/Contents/MacOS/DiffMerge"
        });
}