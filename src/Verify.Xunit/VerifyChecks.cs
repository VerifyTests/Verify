namespace VerifyXunit;

public static class VerifyChecks
{
    public static Task Run()
    {
        var method = UseVerifyAttribute.GetMethod();
        var type = method.ReflectedType!;
        return InnerVerifyChecks.Run(type.Assembly);
    }
}