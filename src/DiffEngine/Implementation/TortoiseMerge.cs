static partial class Implementation
{
    public static DiffTool TortoiseMerge() => new DiffTool(
        name: "TortoiseMerge",
        url: "https://tortoisesvn.net/TortoiseMerge.html",
        supportsAutoRefresh: false,
        isMdi: false,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\TortoiseSVN\bin\TortoiseMerge.exe"
        },
        linuxExePaths: new string[]
        {
        },
        osxExePaths: new string[]
        {
        },
        binaryExtensions: new string[]
        {
        });
}