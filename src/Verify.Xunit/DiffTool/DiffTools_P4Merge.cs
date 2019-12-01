static partial class DiffTools
{
    public static DiffTool P4Merge() => new DiffTool(
        name: "P4Merge",
        url: "https://www.perforce.com/products/helix-core-apps/merge-diff-tool-p4merge",
        argumentPrefix: string.Empty,
        exePaths: new[]
        {
            @"%ProgramFiles%\Perforce\p4merge.exe"
        },
        binaryExtensions: new[]
        {
            "bmp",
            "gif",
            "jpg",
            "jpeg",
            "png",
            "pbm",
            "pgm",
            "ppm",
            "tiff",
            "xbm",
            "xpm"
        });
}