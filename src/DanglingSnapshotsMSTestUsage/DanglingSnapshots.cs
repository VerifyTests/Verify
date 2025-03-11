#pragma warning disable VerifyDanglingSnapshots
[TestClass]
public static class Cleanup
{
    [AssemblyCleanup]
    public static void Run() =>
        DanglingSnapshots.Run();
}