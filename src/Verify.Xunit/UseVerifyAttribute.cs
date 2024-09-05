namespace VerifyXunit;

[EditorBrowsable(EditorBrowsableState.Never)]
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class UseVerifyAttribute :
    BeforeAfterTestAttribute
{
    static AsyncLocal<MethodInfo?> local = new();

    public override void Before(MethodInfo info) =>
        local.Value = info;

    public override void After(MethodInfo info) =>
        local.Value = null;

    public static MethodInfo GetMethod()
    {
        var method = local.Value;
        if (method != null)
        {
            return method;
        }

        throw new("Could not resolve the current test info. This feature uses Verify.Xunit.props to inject `[UseVerify]` on the assembly.");
    }
}