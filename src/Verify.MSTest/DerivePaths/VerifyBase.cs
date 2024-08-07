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
    public static void UseProjectRelativeDirectory(string directory) =>
        Verifier.UseProjectRelativeDirectory(directory);

    /// <summary>
    /// Use a directory relative to the source file directory for storing for `.verified.` files.
    /// </summary>
    public static void UseSourceFileRelativeDirectory(string directory) =>
        Verifier.UseSourceFileRelativeDirectory(directory);
}
