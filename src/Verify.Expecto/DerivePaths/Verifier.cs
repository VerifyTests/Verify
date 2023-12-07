// ReSharper disable UnusedParameter.Local

namespace VerifyExpecto;

public partial class Verifier
{
    static DerivePathInfo derivePathInfo = PathInfo.DeriveDefault;

    internal static PathInfo GetPathInfo(string sourceFile, string typeName, string methodName) =>
        derivePathInfo(sourceFile, VerifierSettings.ProjectDir, typeName, methodName);

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

    /// <summary>
    /// Use a directory relative to the project directory for storing for `.verified.` files.
    /// </summary>
    public static void UseProjectRelativeDirectory(string directory) =>
        DerivePathInfo(
            (sourceFile, projectDirectory, typeName, methodName) => new(
                directory: Path.Combine(projectDirectory, directory),
                typeName: typeName,
                methodName: methodName));
}