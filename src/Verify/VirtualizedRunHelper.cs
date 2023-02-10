class VirtualizedRunHelper
{
    internal static IEnvironment Env { private get; set; } = PhysicalEnvironment.Instance;
    // e.g. WSL or docker run (https://github.com/VerifyTests/Verify#unit-testing-inside-virtualized-environment)
    internal bool AppearsToBeLocalVirtualizedRun { get; private set; }
    internal bool Initialized { get; private set; }

    string originalCodeBaseRootAbsolute = string.Empty;
    string mappedCodeBaseRootAbsolute = string.Empty;

    static readonly char[] separators =
    {
        '\\',
        '/'
    };

    public VirtualizedRunHelper(Assembly userAssembly)
        : this(
            AttributeReader.TryGetSolutionDirectory(userAssembly, false, out var sln) ? sln : string.Empty,
            AttributeReader.TryGetProjectDirectory(userAssembly, false, out var proj) ? proj : string.Empty)
    { }

    internal VirtualizedRunHelper(string solutionDir, string projectDir)
    {
        originalCodeBaseRootAbsolute = string.IsNullOrEmpty(solutionDir) ? projectDir : solutionDir;

        if (!string.IsNullOrEmpty(originalCodeBaseRootAbsolute))
        {
            Initialized =
                TryInitializeFromBuildTimePath(originalCodeBaseRootAbsolute, solutionDir)
                ||
                TryInitializeFromBuildTimePath(originalCodeBaseRootAbsolute, projectDir);
        }
    }

    public string? GetMappedBuildPath(string? path)
    {
        if (path == null ||
            string.IsNullOrEmpty(path))
        {
            return path;
        }

        if (!Initialized)
        {
            // If not initialized (the solution or project dir path might not have been set or they are
            //   not sufficient due to insufficient nesting and hence appearing to have no cross-section
            //   with the current dir)
            TryInitializeFromBuildTimePath(originalCodeBaseRootAbsolute, path);
            Initialized = true;
        }

        if (!AppearsToBeLocalVirtualizedRun)
        {
            return path;
        }

        if (!path.StartsWith(originalCodeBaseRootAbsolute, StringComparison.CurrentCultureIgnoreCase))
        {
            return path;
        }

        var mappedPathRelative = path[originalCodeBaseRootAbsolute.Length..].Replace('\\', '/');

        var mappedPath = Env.CombinePaths(mappedCodeBaseRootAbsolute, mappedPathRelative);

        if (Env.PathExists(mappedPath))
        {
            return mappedPath;
        }

        return path;
    }

    private bool TryInitializeFromBuildTimePath(string originalCodeBaseRoot, string buildTimePath)
    {
        if (Initialized)
        {
            return true;
        }

        if (string.IsNullOrEmpty(buildTimePath))
        {
            return false;
        }

        var appearsBuiltOnCurrentPlatform =
            buildTimePath.Contains(Env.DirectorySeparatorChar) &&
            !buildTimePath.Contains(separators.First(c => c != Env.DirectorySeparatorChar));

        if (appearsBuiltOnCurrentPlatform)
        {
            AppearsToBeLocalVirtualizedRun = false;
            return true;
        }

        var currentDir = Env.CurrentDirectory;
        // First attempt - by the cross-section of the build-time path and run-time path
        if (!string.IsNullOrEmpty(originalCodeBaseRoot))
        {
            var currentDirRelativeToAppRoot = currentDir.TrimStart(separators);
            while (TryRemoveDirFromStartOfPath(ref currentDirRelativeToAppRoot))
            {
                //remove the drive info from the code root
                var mappedCodeBaseRootRelative = originalCodeBaseRoot.Replace('\\', '/');
                while (TryRemoveDirFromStartOfPath(ref mappedCodeBaseRootRelative))
                {
                    if (currentDirRelativeToAppRoot.StartsWith(mappedCodeBaseRootRelative, StringComparison.CurrentCultureIgnoreCase))
                    {
                        mappedCodeBaseRootAbsolute = currentDir[..^currentDirRelativeToAppRoot.Length];

                        // the start of paths to be mapped
                        var codeBaseRootAbsolute = originalCodeBaseRoot[..^mappedCodeBaseRootRelative.Length];

                        var testMappedPath = Env.CombinePaths(
                            mappedCodeBaseRootAbsolute,
                            originalCodeBaseRoot[codeBaseRootAbsolute.Length..].Replace('\\', '/'));

                        if (Env.PathExists(testMappedPath))
                        {
                            originalCodeBaseRootAbsolute = codeBaseRootAbsolute;
                            AppearsToBeLocalVirtualizedRun = true;
                            return true;
                        }
                    }
                }
            }
        }

        // Fallback attempt - via the existence of mapped build-time path within the run-time FS

        // 1) try to get relative if we can (if not, at least cut the first part - or not)
        var buildTimePathRelative = buildTimePath;
        if (!string.IsNullOrEmpty(originalCodeBaseRoot) && buildTimePath.StartsWith(originalCodeBaseRoot, StringComparison.CurrentCultureIgnoreCase))
        {
            buildTimePathRelative = buildTimePathRelative[originalCodeBaseRoot.Length..];
            buildTimePathRelative = buildTimePathRelative.TrimStart(separators);
            if (string.IsNullOrEmpty(buildTimePathRelative.Trim(separators)))
            {
                buildTimePathRelative = buildTimePath;
            }
        }

        // path is actually absolute - let's remove the root
        if (buildTimePathRelative == buildTimePath)
        {
            TryRemoveDirFromStartOfPath(ref buildTimePathRelative);
        }

        // iteratively decrease build-time path from start and try to append to root and check existence
        do
        {
            var codeBaseRootAbsolute = currentDir;
            do
            {
                var testMappedPath = Env.CombinePaths(codeBaseRootAbsolute, buildTimePathRelative.Replace('\\', '/'));
                if (Env.PathExists(testMappedPath))
                {
                    mappedCodeBaseRootAbsolute = codeBaseRootAbsolute;
                    originalCodeBaseRootAbsolute = buildTimePath[..^buildTimePathRelative.Length];
                    AppearsToBeLocalVirtualizedRun = true;
                    return true;
                }

            } while (TryRemoveDirFromEndOfPath(ref codeBaseRootAbsolute));
        } while (TryRemoveDirFromStartOfPath(ref buildTimePathRelative));

        AppearsToBeLocalVirtualizedRun = false;
        return false;
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

    static bool TryRemoveDirFromEndOfPath(ref string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        path = path.TrimEnd(separators);

        int nextSeparatorIdx = path.LastIndexOfAny(separators);
        if (nextSeparatorIdx <= 0 || nextSeparatorIdx == path.Length - 1)
        {
            return false;
        }

        path = path[..nextSeparatorIdx];

        return !string.IsNullOrWhiteSpace(path);
    }
}