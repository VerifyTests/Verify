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
        buildArguments: (path1, path2) => $"--nosplash \"{path1}\" \"{path2}\"",
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