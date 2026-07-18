namespace VerifyTests;

public partial class VerifySettings
{
    /// <summary>
    /// Ignore class arguments in 'verified' filename resulting in the same verified file regardless of class constructor arguments.
    /// Note that the 'received' files still contain the class arguments.
    /// </summary>
    [Obsolete("Use IgnoreConstructorParameters")]
    public void IgnoreClassArguments() =>
        IgnoreConstructorParameters();
}
