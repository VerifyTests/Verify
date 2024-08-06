using Xunit.v3;

namespace VerifyXunit;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class UseVerifyAttribute :
    BeforeAfterTestAttribute
{
    static AsyncLocal<MethodInfo?> local = new();

    public override ValueTask Before(MethodInfo info, IXunitTest test)
    {
         local.Value = info;
         return default;
    }

    public override ValueTask After(MethodInfo info, IXunitTest test)
    {
        local.Value = null;
        return default;
    }

    internal static bool TryGet([NotNullWhen(true)] out MethodInfo? info)
    {
        info = local.Value;
        return info is not null;
    }
}