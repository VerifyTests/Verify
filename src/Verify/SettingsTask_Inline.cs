namespace VerifyTests;

public partial class SettingsTask
{
    /// <inheritdoc cref="VerifySettings.Snapshot(string,string,int,string,string)"/>
    [Pure]
    public SettingsTask Snapshot(
        [StringSyntax("*")][ConstantExpected] string? expected = null,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerArgumentExpression(nameof(expected))] string? expression = null,
        [CallerMemberName] string member = "")
    {
        CurrentSettings.Snapshot(expected, file, line, expression, member);
        return this;
    }

    /// <inheritdoc cref="VerifySettings.NotInline()"/>
    [Pure]
    public SettingsTask NotInline()
    {
        CurrentSettings.NotInline();
        return this;
    }
}
