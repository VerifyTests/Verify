namespace VerifyXunit;

[AttributeUsage(AttributeTargets.Class)]
public class UsesVerifyAttribute :
    BeforeAfterTestAttribute
{
    static AsyncLocal<MethodInfo?> local = new();

    public override void Before(MethodInfo info)
    {
        local.Value = info;
    }

    public override void After(MethodInfo info)
    {
        local.Value = null;
    }

    internal static bool TryGet([NotNullWhen(true)] out MethodInfo? info)
    {
        info = local.Value;
        return info != null;
    }
}