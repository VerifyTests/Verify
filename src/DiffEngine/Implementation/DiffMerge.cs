static partial class Implementation
{
    public static DiffTool DiffMerge() => new DiffTool(
        name: "DiffMerge",
        url: "https://www.sourcegear.com/diffmerge/",
        supportsAutoRefresh: false,
        isMdi: false,
        buildArguments: (path1, path2) => $"--nosplash \"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\SourceGear\Common\DiffMerge\sgdm.exe"
        },
        linuxExePaths: new[]
        {
            "/usr/bin/diffmerge"
        },
        osxExePaths: new[]
        {
            "/Applications/DiffMerge.app/Contents/MacOS/DiffMerge"
        },
        binaryExtensions: new string[]
        {
        });
}