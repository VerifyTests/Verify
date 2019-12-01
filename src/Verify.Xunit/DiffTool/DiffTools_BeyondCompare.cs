static partial class DiffTools
{
    public static DiffTool BeyondCompare() => new DiffTool(
        name: "BeyondCompare",
        url: "https://www.scootersoftware.com/v4help/index.html?command_line_reference.html",
        argumentPrefix: string.Empty,
        exePaths: new[]
        {
            @"%ProgramFiles%\Beyond Compare 4\BCompare.exe"
        },
        binaryExtensions: new[]
        {
            "mp3",
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
            "png",
            "tif",
            "rtf"
        });
}