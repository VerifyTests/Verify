namespace VerifyTests;

public static partial class VerifierSettings
{
    /// <summary>
    /// Ignore class arguments in 'verified' filename resulting in the same verified file regardless of class constructor arguments.
    /// Note that the 'received' files still contain the class arguments.
    /// </summary>
    [Obsolete("Use IgnoreConstructorParameters")]
    public static void IgnoreClassArguments() =>
        IgnoreConstructorParameters();
}
