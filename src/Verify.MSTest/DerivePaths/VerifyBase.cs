// ReSharper disable UnusedParameter.Local

#pragma warning disable VerifyTestsProjectDir
namespace VerifyMSTest;

public partial class VerifyBase
{
    /// <summary>
    /// Use custom path information for `.verified.` files.
    /// </summary>
    /// <remarks>
    /// This is sometimes needed on CI systems that move/remove the original source.
    /// To move to this approach, any existing `.verified.` files will need to be moved to the new directory
    /// </remarks>
    /// <param name="derivePathInfo">Custom callback to control the behavior.</param>
    public static void DerivePathInfo(DerivePathInfo derivePathInfo) =>
        Verifier.DerivePathInfo(derivePathInfo);

    /// <summary>
    /// Use a directory relative to the project directory for storing for `.verified.` files.
    /// </summary>
    [Obsolete("Use the overload that accepts mirrorSourceStructure.")]
    public static void UseProjectRelativeDirectory(string directory) =>
        Verifier.UseProjectRelativeDirectory(directory, false);

    /// <summary>
    /// Use a directory relative to the project directory for storing for `.verified.` files.
    /// </summary>
    /// <param name="directory">The project relative directory to store `.verified.` files in.</param>
    /// <param name="mirrorSourceStructure">
    /// If true, nests `.verified.` files in sub-directories that mirror the directory structure of the test source files relative to the project directory.
    /// </param>
    [OverloadResolutionPriority(1)]
    public static void UseProjectRelativeDirectory(string directory, bool mirrorSourceStructure = false) =>
        Verifier.UseProjectRelativeDirectory(directory, mirrorSourceStructure);

    /// <summary>
    /// Use a directory relative to the source file directory for storing for `.verified.` files.
    /// </summary>
    public static void UseSourceFileRelativeDirectory(string directory) =>
        Verifier.UseSourceFileRelativeDirectory(directory);
}
