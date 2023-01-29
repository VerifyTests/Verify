class VirtualizedRunHelper
{
    // e.g. WSL or docker run (https://github.com/VerifyTests/Verify#unit-testing-inside-virtualized-environment)
    readonly bool appearsToBeLocalVirtualizedRun;
    readonly string originalCodeBaseRootAbsolute = string.Empty;
    readonly string mappedCodeBaseRootAbsolute = string.Empty;
    static readonly char[] Separators = { '\\', '/' };

    public VirtualizedRunHelper(Assembly userAssembly)
    {
        var originalCodeBaseRoot = AttributeReader.TryGetSolutionDirectory(userAssembly, false, out var solutionDir)
            ? solutionDir
            : AttributeReader.GetProjectDirectory(userAssembly);
        var appearsToBeBuiltOnDifferentPlatform =
            !string.IsNullOrEmpty(originalCodeBaseRoot) &&
            !originalCodeBaseRoot.Contains(Path.DirectorySeparatorChar) &&
            originalCodeBaseRoot.Contains("\\");

        if (!appearsToBeBuiltOnDifferentPlatform)
        {
            return;
        }

        string currentDir = Environment.CurrentDirectory;
        string currentDirRelativeToAppRoot = currentDir.TrimStart(Separators);

        // WSL paths mount to /mnt/<drive>/...
        // docker testing mounts to /mnt/approot/...
        if (!TryRemoveDirFromStartOfPath(ref currentDirRelativeToAppRoot) ||
            !TryRemoveDirFromStartOfPath(ref currentDirRelativeToAppRoot))
        {
            return;
        }

        //remove the drive info from the code root
        var mappedCodeBaseRootRelative = originalCodeBaseRoot.Replace('\\', '/');
        if (!TryRemoveDirFromStartOfPath(ref mappedCodeBaseRootRelative))
        {
            return;
        }

        // Move through the code base dir and try to see if it seems to be basePath for currentDir
        while (!currentDirRelativeToAppRoot.StartsWith(mappedCodeBaseRootRelative, StringComparison.CurrentCultureIgnoreCase))
        {
            //no more dirs in code base path - no match found - bail out.
            if (!TryRemoveDirFromStartOfPath(ref mappedCodeBaseRootRelative))
            {
                return;
            }
        }

        mappedCodeBaseRootAbsolute = currentDir[..^currentDirRelativeToAppRoot.Length];

        // the start of paths to be mapped
        originalCodeBaseRootAbsolute = originalCodeBaseRoot[..^mappedCodeBaseRootRelative.Length];

        var testMappedPath = Path.Combine(
            mappedCodeBaseRootAbsolute,
            originalCodeBaseRoot[originalCodeBaseRootAbsolute.Length..].Replace('\\', '/'));

        if (PathExists(testMappedPath))
        {
            appearsToBeLocalVirtualizedRun = true;
        }
    }

    public string? GetMappedBuildPath(string? path)
    {
        if (path == null ||
            string.IsNullOrEmpty(path) ||
            !appearsToBeLocalVirtualizedRun)
        {
            return path;
        }

        if (!path.StartsWith(originalCodeBaseRootAbsolute, StringComparison.CurrentCultureIgnoreCase))
        {
            return path;
        }

        var mappedPathRelative = path[originalCodeBaseRootAbsolute.Length..].Replace('\\', '/');

        var mappedPath = Path.Combine(mappedCodeBaseRootAbsolute, mappedPathRelative);

        if (PathExists(mappedPath))
        {
            return mappedPath;
        }

        return path;
    }

    static bool TryRemoveDirFromStartOfPath(ref string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        path = path.TrimStart(Separators);

        int nextSeparatorIdx = path.IndexOfAny(Separators);
        if (nextSeparatorIdx <= 0 || nextSeparatorIdx == path.Length - 1)
        {
            return false;
        }

        path = path[(nextSeparatorIdx + 1)..];

        return !string.IsNullOrWhiteSpace(path);
    }

    static bool PathExists(string path) =>
        File.Exists(path) || Directory.Exists(path);
}
