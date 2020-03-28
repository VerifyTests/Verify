static partial class Implementation
{
    public static DiffTool Kaleidoscope() => new DiffTool(
        name: "Kaleidoscope",
        url: "https://www.kaleidoscopeapp.com/",
        supportsAutoRefresh: true,
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
            "/Applications/Kaleidoscope.app/Contents/MacOS/ksdiff"
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