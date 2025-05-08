namespace VerifyTests;

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.OnVerifyMismatch(VerifyMismatch)"/>
    [Pure]
    public SettingsTask OnVerifyMismatch(VerifyMismatch verifyMismatch)
    {
        CurrentSettings.OnVerifyMismatch(verifyMismatch);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.OnDelete(VerifyDelete)"/>
    [Pure]
    public SettingsTask OnDelete(VerifyDelete verifyDelete)
    {
        CurrentSettings.OnDelete(verifyDelete);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.OnFirstVerify(FirstVerify)"/>
    [Pure]
    public SettingsTask OnFirstVerify(FirstVerify firstVerify)
    {
        CurrentSettings.OnFirstVerify(firstVerify);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.OnVerify(Action, Action)"/>
    [Pure]
    public SettingsTask OnVerify(Action before, Action after)
    {
        CurrentSettings.OnVerify(before, after);
        return this;
    }
}