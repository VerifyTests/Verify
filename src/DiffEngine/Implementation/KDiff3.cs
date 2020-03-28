static partial class Implementation
{
    public static DiffTool KDiff3() => new DiffTool(
        name: "KDiff3",
        url: "https://github.com/KDE/kdiff3",
        supportsAutoRefresh: false,
        isMdi: false,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\KDiff3\kdiff3.exe"
        },
        linuxExePaths: new string[]
        {
        },
        osxExePaths: new[]
        {
            "/Applications/kdiff3.app/Contents/MacOS/kdiff3"
        },
        binaryExtensions: new string[]
        {
        });
}