// ReSharper disable UnusedParameter.Local

namespace VerifyTests;

public static partial class VerifierSettings
{
    #region defaultDerivePathInfo

    static DerivePathInfo derivePathInfo = (sourceFile, projectDirectory, type, method, methodName) =>
        new(
            directory: Path.GetDirectoryName(sourceFile)!,
            typeName: type.NameWithParent(),
            methodName: methodName);

    #endregion

    internal static PathInfo GetPathInfo(string sourceFile, Type type, MethodInfo method, string methodName) =>
        derivePathInfo(sourceFile, TargetAssembly.ProjectDir, type, method, methodName);

    /// <summary>
    /// Use custom path information for `.verified.` files.
    /// </summary>
    /// <remarks>
    /// This is sometimes needed on CI systems that move/remove the original source.
    /// To move to this approach, any existing `.verified.` files will need to be moved to the new directory
    /// </remarks>
    /// <param name="derivePathInfo">Custom callback to control the behavior.</param>
    public static void DerivePathInfo(DerivePathInfo derivePathInfo) =>
        VerifierSettings.derivePathInfo = derivePathInfo;
}