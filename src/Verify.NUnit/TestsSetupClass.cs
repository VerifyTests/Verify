using NUnit.Framework.Legacy;

[SetUpFixture]
public static class TestsSetupClass
{
    public static Action? action;

    [OneTimeTearDown]
    public static void OneTimeTearDown() =>
        ClassicAssert.False(true);
}