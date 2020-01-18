﻿static partial class DiffTools
{
    static DiffTool BeyondCompare() => new DiffTool(
        name: "BeyondCompare",
        url: "https://www.scootersoftware.com/v4help/index.html?command_line_reference.html",
        shouldTerminate: true,
        supportsAutoRefresh: true,
        isMdi: false,
        buildArguments: pair => $"/solo \"{pair.Received}\" \"{pair.Verified}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Beyond Compare 4\BCompare.exe"
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
            "png",
            "tif",
            "rtf"
        });
}