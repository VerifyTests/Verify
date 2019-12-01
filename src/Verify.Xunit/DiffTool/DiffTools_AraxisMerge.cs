static partial class DiffTools
{
    public static DiffTool AraxisMerge() => new DiffTool(
        name: "AraxisMerge",
        url: "https://www.araxis.com/merge",
        argumentPrefix: "/nowait ",
        exePaths: new[]
        {
            @"%ProgramFiles%\Araxis\Araxis Merge\Compare.exe"
        },
        binaryExtensions: new[]
        {
            //https://www.araxis.com/merge/documentation-windows/comparing-image-files.en
            "gif",
            "jif",
            "jpeg",
            "jpg",
            "j2c",
            "j2k",
            "jp2",
            "jpc",
            "jpx",
            "pbm",
            "pgm",
            "png",
            "ppm",
            "ras",
            "tif",
            "tiff",
            "tga",
            "emf",
            "wmf",
            "bmp",
            "dib",
            "pcx",
        });
}