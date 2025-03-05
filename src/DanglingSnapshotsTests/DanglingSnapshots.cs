[SetUpFixture]
static class DanglingSnapshots
{
    [OneTimeTearDown]
    public static void Run() =>
        DanglingSnapshotsCheck.Run();
}