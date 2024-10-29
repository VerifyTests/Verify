namespace VerifyTests;

public partial class SettingsTask
{
    [Pure]
    public SettingsTask OnVerifyMismatch(VerifyMismatch verifyMismatch)
    {
        CurrentSettings.OnVerifyMismatch(verifyMismatch);
        return this;
    }

    [Pure]
    public SettingsTask OnDelete(VerifyDelete verifyDelete)
    {
        CurrentSettings.OnDelete(verifyDelete);
        return this;
    }

    [Pure]
    public SettingsTask OnFirstVerify(FirstVerify firstVerify)
    {
        CurrentSettings.OnFirstVerify(firstVerify);
        return this;
    }

    [Pure]
    public SettingsTask OnVerify(Action before, Action after)
    {
        CurrentSettings.OnVerify(before, after);
        return this;
    }
}