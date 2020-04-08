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
        buildWindowsArguments: (tempFile, targetFile) => $"/nowait \"{tempFile}\" \"{targetFile}\"",
        buildLinuxArguments: null,
        buildOsxArguments: (tempFile, targetFile) => $"-nowait \"{tempFile}\" \"{targetFile}\"",
        windowsExePaths: new[]
        {
            @"%ProgramFiles%\Araxis\Araxis Merge\Compare.exe"
        },
        linuxExePaths: Array.Empty<string>(),
        osxExePaths: new[]
        {
            "/Applications/Araxis Merge.app/Contents/Utilities/compare"
        },
        binaryExtensions: new[]
        {
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
        notes: @"
 * [Supported image files](https://www.araxis.com/merge/documentation-windows/comparing-image-files.en)
 * [Windows command line usage](https://www.araxis.com/merge/documentation-windows/command-line.en)
 * [MacOS command line usage](https://www.araxis.com/merge/documentation-os-x/command-line.en)
 * [Installing MacOS command line](https://www.araxis.com/merge/documentation-os-x/installing.en)");
}