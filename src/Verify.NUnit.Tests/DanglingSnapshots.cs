[SetUpFixture]
static class DanglingSnapshots
{
    [ModuleInitializer]
    public static void Init() =>
        Environment.SetEnvironmentVariable("CI", "true");

    [OneTimeTearDown]
    public static void Run() =>
        DanglingSnapshotsCheck.Run();
}