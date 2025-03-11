#pragma warning disable VerifyDanglingSnapshots

[SetUpFixture]
public static class SetUp
{
    [OneTimeTearDown]
    public static void Run() =>
        DanglingSnapshots.Run();
}