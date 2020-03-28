static partial class Implementation
{
    public static DiffTool BeyondCompare() => new DiffTool(
        name: "BeyondCompare",
        url: "https://www.scootersoftware.com/v4help/index.html?command_line_reference.html",
        supportsAutoRefresh: true,
        isMdi: false,
        buildArguments: (path1, path2) => $"/solo \"{path1}\" \"{path2}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Beyond Compare 4\BCompare.exe",
            @"%ProgramFiles%\Beyond Compare 3\BCompare.exe"
        },
        linuxExePaths: new[]
        {
            //TODO:
            "/usr/lib/beyondcompare/bcomp"
        },
        osxExePaths: new[]
        {
            "/Applications/Beyond Compare.app/Contents/MacOS/bcomp"
        },
        binaryExtensions: new[]
        {
            "mp3", //?
            "xls",
            "xlsm",
            "xlsx",
            "doc",
            "docm",
            "docx",
            "dot",
            "dotm",
            "dotx",
            "pdf",
            "bmp",
            "gif",
            "ico",
            "jpg",
            "jpeg",
            "png",
            "tif",
            "tiff",
            "rtf"
        });
}