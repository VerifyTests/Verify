using NUnit.Framework.Legacy;

[SetUpFixture]
public static class TestsSetupClass
{
    public static Action? action;

    [OneTimeTearDown]
    public static void OneTimeTearDown()
    {
        var adapter = TestContext.CurrentContext.Test;
        var testMethod = adapter.GetTestMethod();
        var type = testMethod.TypeInfo.Type;
        InnerVerifyChecks.Complete();
    }
}