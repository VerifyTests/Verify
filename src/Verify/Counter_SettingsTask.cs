namespace VerifyTests;

public partial class SettingsTask
{
#if NET6_0_OR_GREATER

    [Pure]
    public SettingsTask AddNamedDate(Date value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        CurrentSettings.AddNamedDate(value, name);
        return this;
    }

    [Pure]
    public SettingsTask AddNamedTime(Time value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        CurrentSettings.AddNamedTime(value, name);
        return this;
    }

#endif

    [Pure]
    public SettingsTask AddNamedDateTime(DateTime value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        CurrentSettings.AddNamedDateTime(value, name);
        return this;
    }

    [Pure]
    public SettingsTask AddNamedDateTimeOffset(DateTimeOffset value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        CurrentSettings.AddNamedDateTimeOffset(value, name);
        return this;
    }

    [Pure]
    public SettingsTask AddNamedGuid(Guid value, [CallerArgumentExpression(nameof(value))] string name = "")
    {
        CurrentSettings.AddNamedGuid(value, name);
        return this;
    }
}