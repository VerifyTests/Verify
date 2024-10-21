// ReSharper disable ReturnTypeCanBeNotNullable
namespace VerifyTests;

public partial class SettingsTask
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() =>
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        base.GetHashCode();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string? ToString() =>
        base.ToString();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) =>
        // ReSharper disable once BaseObjectEqualsIsObjectEquals
        base.Equals(obj);
}