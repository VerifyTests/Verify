using System;
using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition AraxisMerge() => new ToolDefinition(
        name: DiffTool.AraxisMerge,
        url: "https://www.araxis.com/merge",
        supportsAutoRefresh: true,
        isMdi: true,
        supportsText: true,
        //https://www.araxis.com/merge/documentation-windows/command-line.en
        //https://www.araxis.com/merge/documentation-os-x/command-line.en
        buildArguments: (tempFile, targetFile) => $"-nowait \"{tempFile}\" \"{targetFile}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Araxis\Araxis Merge\Compare.exe"
        },
        binaryExtensions: new[]
        {
            //https://www.araxis.com/merge/documentation-windows/comparing-image-files.en
            "bmp",
            "dib",
            "emf",
            "gif",
            "jif",
            "j2c",
            "j2k",
            "jp2",
            "jpc",
            "jpeg",
            "jpg",
            "jpx",
            "pbm", //?
            "pcx",
            "pgm",
            "png",
            "ppm", //?
            "ras", //?
            "tif",
            "tiff",
            "tga",
            "wmf", //?
        },
        linuxExePaths: Array.Empty<string>(),
        osxExePaths: new[]
        {
            //https://www.araxis.com/merge/documentation-os-x/installing.en
            "/Applications/Araxis Merge.app/Contents/Utilities/compare"
        });
}