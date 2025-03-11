[SetUpFixture]
static class SetUp
{
    [OneTimeTearDown]
    public static void Run() =>
#pragma warning disable VerifyDanglingSnapshots
        DanglingSnapshots.Run();
#pragma warning restore VerifyDanglingSnapshots
}