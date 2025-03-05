#pragma warning disable InnerVerifyChecks
namespace VerifyNUnit;

public static class VerifyChecks
{
    public static Task Run()
    {
        var adapter = TestContext.CurrentContext.Test;
        var testMethod = adapter.GetTestMethod();
        var type = testMethod.TypeInfo.Type;
        return InnerVerifyChecks.Run(type.Assembly);
    }
}