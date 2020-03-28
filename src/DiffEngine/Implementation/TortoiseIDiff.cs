static partial class Implementation
{
    public static DiffTool TortoiseIDiff() => new DiffTool(
        name: "TortoiseIDiff",
        url: "https://tortoisesvn.net/TortoiseIDiff.html",
        supportsAutoRefresh: false,
        isMdi: false,
        buildArguments: (path1, path2) => $"/left:\"{path1}\" /right:\"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\TortoiseSVN\bin\TortoiseIDiff.exe"
        },
        linuxExePaths: new string[]
        {
        },
        osxExePaths: new string[]
        {
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
        });
}