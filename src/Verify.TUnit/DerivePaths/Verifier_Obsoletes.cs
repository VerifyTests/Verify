namespace VerifyTUnit;

public partial class Verifier
{
    /// <summary>
    /// Use a directory relative to the project directory for storing for `.verified.` files.
    /// </summary>
    [Obsolete("Use the overload that accepts mirrorSourceStructure.")]
    public static void UseProjectRelativeDirectory(string directory) =>
        UseProjectRelativeDirectory(directory, false);
}
