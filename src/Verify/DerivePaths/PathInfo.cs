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
        Guard.BadDirectoryName(directory, nameof(directory));
        Guard.BadFileNameNullable(typeName, nameof(typeName));
        Guard.BadFileNameNullable(methodName, nameof(methodName));
        TypeName = typeName;
        MethodName = methodName;
        Directory = directory;
    }

    #region defaultDerivePathInfo

    internal static PathInfo DeriveDefault(
        string sourceFile,
        string projectDirectory,
        Type type,
        MethodInfo method) =>
        new(
            directory: IoHelpers.GetDirectoryName(sourceFile)!,
            typeName: type.NameWithParent(),
            methodName: method.Name);
    #endregion

    internal static PathInfo DeriveDefault(
        string sourceFile,
        string projectDirectory,
        string typeName,
        string methodName) =>
        new(
            directory: IoHelpers.GetDirectoryName(sourceFile)!,
            typeName: typeName,
            methodName: methodName);
}