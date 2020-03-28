static partial class Implementation
{
    public static DiffTool Kaleidoscope() => new DiffTool(
        name: "Kaleidoscope",
        url: "https://www.kaleidoscopeapp.com/",
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
            "/usr/local/bin/ksdiff"
        },
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
        });
}