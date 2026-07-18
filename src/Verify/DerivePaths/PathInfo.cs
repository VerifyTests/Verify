namespace VerifyTests;

[DebuggerDisplay("Directory = {Directory} | TypeName = {TypeName} | MethodName = {MethodName}")]
public readonly struct PathInfo
{
    public string? Directory { get; }
    public string? TypeName { get; }
    public string? MethodName { get; }

    public PathInfo(
        string? directory = null,
        string? typeName = null,
        string? methodName = null)
    {
        Guards.BadDirectoryName(directory);

        if (typeName is not (null or ""))
        {
            Guards.BadFileName(typeName);
        }

        if (methodName is not null)
        {
            Guards.BadFileName(methodName);
        }

        TypeName = typeName;
        MethodName = methodName;
        Directory = directory;
    }

    #region defaultDerivePathInfo

    public static PathInfo DeriveDefault(
        string sourceFile,
        string projectDirectory,
        Type type,
        MethodInfo method) =>
        new(
            directory: IoHelpers.ResolveDirectoryFromSourceFile(sourceFile),
            typeName: type.NameWithParent(),
            methodName: method.Name);

    #endregion

    /// <summary>
    /// Derives a <see cref="PathInfo" /> that stores `.verified.` files in <paramref name="directory" /> relative to
    /// <paramref name="projectDirectory" />. When <paramref name="mirrorSourceStructure" /> is true, the files are
    /// nested in sub-directories that mirror the directory structure of the test source file relative to the project
    /// directory. Source files at, above, or outside the project directory fall back to <paramref name="directory" />.
    /// </summary>
    public static PathInfo DeriveProjectRelative(
        string directory,
        bool mirrorSourceStructure,
        string sourceFile,
        string projectDirectory,
        string? typeName,
        string? methodName)
    {
        var resolved = Path.Combine(projectDirectory, directory);
        if (mirrorSourceStructure)
        {
            var sourceDirectory = Path.GetDirectoryName(sourceFile)!;
            var relative = Path.GetRelativePath(projectDirectory, sourceDirectory);
            if (relative != "." &&
                !relative.StartsWith("..", StringComparison.Ordinal) &&
                !Path.IsPathRooted(relative))
            {
                resolved = Path.Combine(resolved, relative);
            }
        }

        return new(
            directory: resolved,
            typeName: typeName,
            methodName: methodName);
    }

    internal static PathInfo DeriveDefault(
        string sourceFile,
        string projectDirectory,
        string typeName,
        string methodName) =>
        new(
            directory: IoHelpers.ResolveDirectoryFromSourceFile(sourceFile),
            typeName: typeName,
            methodName: methodName);
}