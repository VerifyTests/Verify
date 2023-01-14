// ReSharper disable UnusedParameter.Local

namespace VerifyNUnit;

public partial class Verifier
{
    static DerivePathInfo derivePathInfo = PathInfo.DeriveDefault;

    internal static PathInfo GetPathInfo(string sourceFile, Type type, MethodInfo method) =>
        derivePathInfo(sourceFile, TargetAssembly.ProjectDir, type, method);

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