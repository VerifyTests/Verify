class VirtualizedRunHelper
{
    // e.g. WSL or docker run (https://github.com/VerifyTests/Verify#unit-testing-inside-virtualized-environment)
    internal bool AppearsToBeLocalVirtualizedRun { get; }

    string _originalCodeBaseRootAbsolute = string.Empty;
    string _mappedCodeBaseRootAbsolute = string.Empty;
    readonly Func<string,bool> _pathExists;

    static readonly char[] separators =
    {
        '\\',
        '/'
    };

    public VirtualizedRunHelper(Assembly userAssembly)
        : this(GetOriginalCodeBaseRoot(userAssembly),
            Environment.CurrentDirectory,
            Path.DirectorySeparatorChar,
            path => File.Exists(path) || Directory.Exists(path))
    {
    }

    private static string GetOriginalCodeBaseRoot(Assembly userAssembly) =>
        AttributeReader.TryGetSolutionDirectory(userAssembly, false, out var solutionDir)
            ? solutionDir : AttributeReader.GetProjectDirectory(userAssembly);

    internal VirtualizedRunHelper(string originalCodeBaseRoot, string currentDir, char runtimeSeparator, Func<string, bool> pathExists)
    {
        _pathExists = pathExists;

       var appearsBuiltOnDifferentPlatform =
            !string.IsNullOrEmpty(originalCodeBaseRoot) &&
            !originalCodeBaseRoot.Contains(runtimeSeparator) &&
            originalCodeBaseRoot.Contains("\\");

        if (!appearsBuiltOnDifferentPlatform)
        {
            return;
        }

        var currentDirRelativeToAppRoot = currentDir.TrimStart(separators);
        while (TryRemoveDirFromStartOfPath(ref currentDirRelativeToAppRoot))
        {
            //remove the drive info from the code root
            var mappedCodeBaseRootRelative = originalCodeBaseRoot.Replace('\\', '/');
            while (TryRemoveDirFromStartOfPath(ref mappedCodeBaseRootRelative))
            {
                if (currentDirRelativeToAppRoot.StartsWith(mappedCodeBaseRootRelative, StringComparison.CurrentCultureIgnoreCase))
                {
                    _mappedCodeBaseRootAbsolute = currentDir[..^currentDirRelativeToAppRoot.Length];

                    // the start of paths to be mapped
                    _originalCodeBaseRootAbsolute = originalCodeBaseRoot[..^mappedCodeBaseRootRelative.Length];

                    var testMappedPath = Path.Combine(
                        _mappedCodeBaseRootAbsolute,
                        originalCodeBaseRoot[_originalCodeBaseRootAbsolute.Length..].Replace('\\', '/'));

                    if (pathExists(testMappedPath))
                    {
                        AppearsToBeLocalVirtualizedRun = true;
                    }
                }
            }
        }
    }

    public string? GetMappedBuildPath(string? path)
    {
        if (path == null ||
            string.IsNullOrEmpty(path) ||
            !AppearsToBeLocalVirtualizedRun)
        {
            return path;
        }

        if (!path.StartsWith(_originalCodeBaseRootAbsolute, StringComparison.CurrentCultureIgnoreCase))
        {
            return path;
        }

        var mappedPathRelative = path[_originalCodeBaseRootAbsolute.Length..].Replace('\\', '/');

        var mappedPath = Path.Combine(_mappedCodeBaseRootAbsolute, mappedPathRelative);

        if (_pathExists(mappedPath))
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

        path = path.TrimStart(separators);

        int nextSeparatorIdx = path.IndexOfAny(separators);
        if (nextSeparatorIdx <= 0 || nextSeparatorIdx == path.Length - 1)
        {
            return false;
        }

        path = path[(nextSeparatorIdx + 1)..];

        return !string.IsNullOrWhiteSpace(path);
    }
}