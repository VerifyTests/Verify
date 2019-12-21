using System;

static partial class DiffTools
{
    public static DiffTool AraxisMerge() => new DiffTool(
        name: "AraxisMerge",
        url: "https://www.araxis.com/merge",
        argumentPrefix: "/nowait ",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Araxis\Araxis Merge\Compare.exe"
        },
        osxExePaths: new[]
        {
            "/Applications/Araxis Merge.app/Contents/MacOS/Araxis Merge"
        },
        linuxExePaths: Array.Empty<string>(),
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
            "ppm",//?
            "ras",//?
            "tif",
            "tiff",
            "tga",
            "wmf", //?
        });
}