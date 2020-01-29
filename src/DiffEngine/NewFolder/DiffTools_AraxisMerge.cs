using System;

public static partial class DiffTools
{
    static DiffTool AraxisMerge() => new DiffTool(
        name: "AraxisMerge",
        url: "https://www.araxis.com/merge",
        supportsAutoRefresh: true,
        isMdi: true,
        buildArguments: (path1, path2) => $"/nowait \"{path1}\" \"{path2}\"",
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
            "ppm", //?
            "ras", //?
            "tif",
            "tiff",
            "tga",
            "wmf", //?
        });
}