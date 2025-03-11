[TestClass]
public static class Cleanup
{
    [AssemblyCleanup]
    public static void Run() =>
#pragma warning disable VerifyDanglingSnapshots
        DanglingSnapshots.Run();
#pragma warning restore VerifyDanglingSnapshots
}