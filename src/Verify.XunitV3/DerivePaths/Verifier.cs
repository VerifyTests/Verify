// ReSharper disable UnusedParameter.Local

#pragma warning disable VerifyTestsProjectDir
namespace VerifyXunit;

public partial class Verifier
{
    static DerivePathInfo derivePathInfo = PathInfo.DeriveDefault;

    internal static PathInfo GetPathInfo(string sourceFile, Type type, MethodInfo method) =>
        derivePathInfo(sourceFile, VerifierSettings.ProjectDir, type, method);

    /// <summary>
    /// Use custom path information for `.verified.` files.
    /// </summary>
    /// <remarks>
    /// This is sometimes needed on CI systems that move/remove the original source.
    /// To move to this approach, any existing `.verified.` files will need to be moved to the new directory
    /// </remarks>
    /// <param name="derivePathInfo">Custom callback to control the behavior.</param>
    public static void DerivePathInfo(DerivePathInfo derivePathInfo)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        Verifier.derivePathInfo = derivePathInfo;
    }

    /// <summary>
    /// Use a directory relative to the project directory for storing `.verified.` files.
    /// </summary>
    /// <param name="directory">The project relative directory to store `.verified.` files in.</param>
    /// <param name="mirrorSourceStructure">
    /// If true, nests `.verified.` files in sub-directories that mirror the directory structure of the test source files relative to the project directory.
    /// </param>
    [OverloadResolutionPriority(1)]
    public static void UseProjectRelativeDirectory(string directory, bool mirrorSourceStructure = false) =>
        DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
                PathInfo.DeriveProjectRelative(directory, mirrorSourceStructure, sourceFile, projectDirectory, type.NameWithParent(), method.Name));

    /// <summary>
    /// Use a directory relative to the source file directory for storing `.verified.` files.
    /// </summary>
    public static void UseSourceFileRelativeDirectory(string directory) =>
        DerivePathInfo(
            (sourceFile, projectDirectory, type, method) => new(
                directory: Path.Combine(Path.GetDirectoryName(sourceFile)!, directory),
                typeName: type.NameWithParent(),
                methodName: method.Name));
}