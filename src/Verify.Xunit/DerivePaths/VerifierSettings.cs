// ReSharper disable UnusedParameter.Local

namespace VerifyXunit;

public partial class Verifier
{
    #region defaultDerivePathInfo

    static DerivePathInfo derivePathInfo = (sourceFile, projectDirectory, type, method) =>
        new(
            directory: Path.GetDirectoryName(sourceFile)!,
            typeName: type,
            methodName: method);

    #endregion

    internal static PathInfo GetPathInfo(string sourceFile, string typeName, string methodName) =>
        derivePathInfo(sourceFile, TargetAssembly.ProjectDir, typeName, methodName);

    /// <summary>
    /// Use custom path information for `.verified.` files.
    /// </summary>
    /// <remarks>
    /// This is sometimes needed on CI systems that move/remove the original source.
    /// To move to this approach, any existing `.verified.` files will need to be moved to the new directory
    /// </remarks>
    /// <param name="derivePathInfo">Custom callback to control the behavior.</param>
    public static void DerivePathInfo(DerivePathInfo derivePathInfo) =>
        Verifier.derivePathInfo = derivePathInfo;
}