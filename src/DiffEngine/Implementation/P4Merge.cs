using DiffEngine;

static partial class Implementation
{
    public static ToolDefinition P4Merge() => new ToolDefinition(
        name: DiffTool.P4Merge,
        url: "https://www.perforce.com/products/helix-core-apps/merge-diff-tool-p4merge",
        supportsAutoRefresh: false,
        isMdi: false,
        supportsText: true,
        buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"", 
        windowsExePaths: new[]
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
            "tif",
            "tiff",
            "xbm",
            "xpm"
        },
        linuxExePaths: new[]
        {
            @"/usr/bin/p4merge"
        },
        osxExePaths: new[]
        {
            @"/Applications/p4merge.app/Contents/MacOS/p4merge"
        });
}