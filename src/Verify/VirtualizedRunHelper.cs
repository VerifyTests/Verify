class VirtualizedRunHelper
{
    // e.g. WSL or docker run (https://github.com/VerifyTests/Verify#unit-testing-inside-virtualized-environment)
    private readonly bool appearsToBeLocalVirtualizedRun;
    private readonly string originalCodeBaseRootAbsolute = string.Empty;
    private readonly string mappedCodeBaseRootAbsolute = string.Empty;
    private static readonly char[] Separators = { '\\', '/' };

    public VirtualizedRunHelper(Assembly userAssembly)
    {
        string? solutionDir;
        string originalCodeBaseRoot = AttributeReader.TryGetSolutionDirectory(userAssembly, false, out solutionDir)
            ? solutionDir
            : AttributeReader.GetProjectDirectory(userAssembly);
        bool appersToBeBuiltOnDiferentPlatform =
            !string.IsNullOrEmpty(originalCodeBaseRoot) &&
            !originalCodeBaseRoot.Contains(Path.DirectorySeparatorChar) &&
            originalCodeBaseRoot.Contains("\\");

        if (!appersToBeBuiltOnDiferentPlatform)
        {
            return;
        }

        string currentDir = Environment.CurrentDirectory;
        string currentDirRelativeToAppRoot = currentDir.TrimStart(Separators);

        // WSL paths mount to /mnt/<drive>/...
        // docker testing mounts to /mnt/approot/...
        if (!TryRemoveDirFromStartOfPath(ref currentDirRelativeToAppRoot) || !TryRemoveDirFromStartOfPath(ref currentDirRelativeToAppRoot))
        {
            return;
        }

        //remove the drive info from the code root
        string mappedCodeBaseRootRelative = originalCodeBaseRoot.Replace('\\', '/');
        if (!TryRemoveDirFromStartOfPath(ref mappedCodeBaseRootRelative))
        {
            return;
        }


        // Move through the code base dir and try to see if it seems to be basepath for currentdir
        while (!currentDirRelativeToAppRoot.StartsWith(mappedCodeBaseRootRelative, StringComparison.CurrentCultureIgnoreCase))
        {
            //no more dirs in code base path - no match found - bail out.
            if (!TryRemoveDirFromStartOfPath(ref mappedCodeBaseRootRelative))
            {
                return;
            }
        }

        mappedCodeBaseRootAbsolute = currentDir.Substring(0, currentDir.Length - currentDirRelativeToAppRoot.Length);

        // the start of paths to be mapped
        originalCodeBaseRootAbsolute = originalCodeBaseRoot
            .Substring(0, originalCodeBaseRoot.Length - mappedCodeBaseRootRelative.Length);

        string testMappedPath = Path.Combine(
            mappedCodeBaseRootAbsolute,
            originalCodeBaseRoot.Substring(originalCodeBaseRootAbsolute.Length).Replace('\\', '/'));

        if (!PathExists(testMappedPath))
        {
            return;
        }

        appearsToBeLocalVirtualizedRun = true;
    }

    public string? GetMappedBuildPath(string? path)
    {
        if (path == null || string.IsNullOrEmpty(path) || !appearsToBeLocalVirtualizedRun)
        {
            return path;
        }

        if (!path.StartsWith(originalCodeBaseRootAbsolute, StringComparison.CurrentCultureIgnoreCase))
        {
            return path;
        }

        string mappedPathRelative = path.Substring(originalCodeBaseRootAbsolute.Length).Replace('\\', '/');

        string mappedPath = Path.Combine(mappedCodeBaseRootAbsolute, mappedPathRelative);

        if (!PathExists(mappedPath))
        {
            return path;
        }

        return mappedPath;
    }

    public string? GetDirectoryName(string? path) =>
        Path.GetDirectoryName(GetMappedBuildPath(path));

    private static bool TryRemoveDirFromStartOfPath(ref string path)
    {
        //result = string.Empty;

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

        path = path.Substring(nextSeparatorIdx + 1);

        return !string.IsNullOrWhiteSpace(path);
    }

    private static bool PathExists(string path) =>
        File.Exists(path) || Directory.Exists(path);
}
