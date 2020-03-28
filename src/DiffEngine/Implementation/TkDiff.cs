static partial class Implementation
{
    public static DiffTool TkDiff() => new DiffTool(
        name: "TkDiff",
        url: "https://sourceforge.net/projects/tkdiff/",
        supportsAutoRefresh: false,
        isMdi: false,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: new string[]
        {
        },
        linuxExePaths: new string[]
        {
        },
        osxExePaths: new[]
        {
            "/Applications/TkDiff.app/Contents/MacOS/tkdiff"
        },
        binaryExtensions: new string[]
        {
        });
}