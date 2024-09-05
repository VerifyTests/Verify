#pragma warning disable InnerVerifyChecks
namespace VerifyXunit;

public static class VerifyChecks
{
    public static Task Run()
    {
        var method = TestContext.Current.GetTestMethod();
        var type = method.Method.DeclaringType!;
        return InnerVerifyChecks.Run(type.Assembly);
    }
}