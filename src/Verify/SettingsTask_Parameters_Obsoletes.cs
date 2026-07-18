namespace VerifyTests;

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.IgnoreClassArguments()"/>
    [Obsolete("Use IgnoreConstructorParameters")]
    [Pure]
    public SettingsTask IgnoreClassArguments()
    {
        CurrentSettings.IgnoreConstructorParameters();
        return this;
    }
}
