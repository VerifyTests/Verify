class VirtualizedRunHelper
{
    internal static IEnvironment Env { private get; set; } = PhysicalEnvironment.Instance;
    // e.g. WSL or docker run (https://github.com/VerifyTests/Verify#unit-testing-inside-virtualized-environment)
    internal bool AppearsToBeLocalVirtualizedRun { get; private set; }
    internal bool Initialized { get; private set; }

    string originalCodeBaseRootAbsolute;
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
                TryInitializeFromBuildTimePath(originalCodeBaseRootAbsolute, solutionDir) ||
                TryInitializeFromBuildTimePath(originalCodeBaseRootAbsolute, projectDir);
        }
    }

    public string GetMappedBuildPath(string path)
    {
        if (!Initialized &&
            !path.Equals(originalCodeBaseRootAbsolute, StringComparison.CurrentCultureIgnoreCase))
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

    bool TryInitializeFromBuildTimePath(string originalCodeBaseRoot, string buildTimePath)
    {
        if (Initialized)
        {
            return true;
        }

        if (string.IsNullOrEmpty(buildTimePath))
        {
            return false;
        }

        if (AppearsBuiltOnCurrentPlatform(buildTimePath))
        {
            AppearsToBeLocalVirtualizedRun = false;
            return true;
        }

        // First attempt - by the cross-section of the build-time path and run-time path
        if (!string.IsNullOrEmpty(originalCodeBaseRoot))
        {
            if (TryFindByCrossSectionOfBuildRunPath(originalCodeBaseRoot, out var crossMapped, out var crossOriginal))
            {
                mappedCodeBaseRootAbsolute = crossMapped;
                originalCodeBaseRootAbsolute = crossOriginal;
                AppearsToBeLocalVirtualizedRun = true;
                return true;
            }
        }

        // Fallback attempt - via the existence of mapped build-time path within the run-time FS

        // 1) try to get relative if we can (if not, at least cut the first part - or not)
        if (TryGetRelative(originalCodeBaseRoot, buildTimePath, out var relativeMapped, out var relativeOriginal))
        {
            mappedCodeBaseRootAbsolute = relativeMapped;
            originalCodeBaseRootAbsolute = relativeOriginal;
            AppearsToBeLocalVirtualizedRun = true;
            return true;
        }

        AppearsToBeLocalVirtualizedRun = false;
        return false;
    }

    bool TryGetRelative(
        string originalCodeBaseRoot,
        string buildTimePath,
        [NotNullWhen(true)] out string? codeBaseRootAbsolute,
        [NotNullWhen(true)] out string? baseRootAbsolute)
    {
        var buildTimePathRelative = GetBuildTimePathRelative(originalCodeBaseRoot, buildTimePath);

        // iteratively decrease build-time path from start and try to append to root and check existence
        do
        {
            codeBaseRootAbsolute = Env.CurrentDirectory;
            do
            {
                var testMappedPath = Env.CombinePaths(codeBaseRootAbsolute, buildTimePathRelative.Replace('\\', '/'));
                if (Env.PathExists(testMappedPath))
                {
                    baseRootAbsolute = buildTimePath[..^buildTimePathRelative.Length];
                    mappedCodeBaseRootAbsolute = codeBaseRootAbsolute;
                    return true;
                }
            } while (TryRemoveDirFromEndOfPath(ref codeBaseRootAbsolute));
        } while (TryRemoveDirFromStartOfPath(ref buildTimePathRelative));

        codeBaseRootAbsolute = null;
        baseRootAbsolute = null;
        return false;
    }

    static bool TryFindByCrossSectionOfBuildRunPath(
        string originalCodeBaseRoot,
        [NotNullWhen(true)] out string? mappedCodeBaseRootAbsolute,
        [NotNullWhen(true)] out string? codeBaseRootAbsolute)
    {
        var currentDirRelativeToAppRoot = Env.CurrentDirectory.TrimStart(separators);
        //remove the drive info from the code root
        var mappedCodeBaseRootRelative = originalCodeBaseRoot.Replace('\\', '/');
        while (TryRemoveDirFromStartOfPath(ref currentDirRelativeToAppRoot))
        {
            while (TryRemoveDirFromStartOfPath(ref mappedCodeBaseRootRelative))
            {
                if (!currentDirRelativeToAppRoot.StartsWith(mappedCodeBaseRootRelative, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                mappedCodeBaseRootAbsolute = Env.CurrentDirectory[..^currentDirRelativeToAppRoot.Length];

                // the start of paths to be mapped

                codeBaseRootAbsolute = originalCodeBaseRoot[..^mappedCodeBaseRootRelative.Length];

                var testMappedPath = Env.CombinePaths(
                    mappedCodeBaseRootAbsolute,
                    originalCodeBaseRoot[codeBaseRootAbsolute.Length..].Replace('\\', '/'));

                if (Env.PathExists(testMappedPath))
                {
                    return true;
                }
            }
        }

        mappedCodeBaseRootAbsolute = null;
        codeBaseRootAbsolute = null;
        return false;
    }

    static string GetBuildTimePathRelative(string originalCodeBaseRoot, string buildTimePath)
    {
        var buildTimePathRelative = buildTimePath;
        if (!string.IsNullOrEmpty(originalCodeBaseRoot) &&
            buildTimePath.StartsWith(originalCodeBaseRoot, StringComparison.CurrentCultureIgnoreCase))
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

        return buildTimePathRelative;
    }

    static bool AppearsBuiltOnCurrentPlatform(string buildTimePath) =>
        buildTimePath.Contains(Env.DirectorySeparatorChar) &&
        !buildTimePath.Contains(separators.First(_ => _ != Env.DirectorySeparatorChar));

    static bool TryRemoveDirFromStartOfPath(ref string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        path = path.TrimStart(separators);

        var nextSeparatorIdx = path.IndexOfAny(separators);
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

        var nextSeparatorIdx = path.LastIndexOfAny(separators);
        if (nextSeparatorIdx <= 0 ||
            nextSeparatorIdx == path.Length - 1)
        {
            return false;
        }

        path = path[..nextSeparatorIdx];

        return !string.IsNullOrWhiteSpace(path);
    }
}