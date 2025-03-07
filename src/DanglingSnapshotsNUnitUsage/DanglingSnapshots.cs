[SetUpFixture]
static class SetUp
{
    [OneTimeTearDown]
    public static void Run() =>
        DanglingSnapshots.Run();
}