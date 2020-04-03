using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition BeyondCompare() => new ToolDefinition(
        name: DiffTool.BeyondCompare,
        url: "https://www.scootersoftware.com/v4help/index.html?command_line_reference.html",
        supportsAutoRefresh: true,
        isMdi: false,
        supportsText: true,
        buildArguments: (tempFile, targetFile) => $"/solo \"{tempFile}\" \"{targetFile}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Beyond Compare 4\BCompare.exe",
            @"%ProgramFiles%\Beyond Compare 3\BCompare.exe"
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
        },
        linuxExePaths: new[]
        {
            //TODO:
            "/usr/lib/beyondcompare/bcomp"
        },
        osxExePaths: new[]
        {
            "/Applications/Beyond Compare.app/Contents/MacOS/bcomp"
        });
}