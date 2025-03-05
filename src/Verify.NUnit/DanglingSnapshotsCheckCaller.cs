#pragma warning disable DanglingSnapshotsCheck

[SetUpFixture]
static class DanglingSnapshotsCheckCaller
{
    [OneTimeTearDown]
    public static void OneTimeTearDown() =>
        DanglingSnapshotsCheck.Run();
}