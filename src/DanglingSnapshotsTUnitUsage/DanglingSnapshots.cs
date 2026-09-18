#pragma warning disable VerifyDanglingSnapshots

public static class Cleanup
{
    [After(TestSession)]
    public static void Run() =>
        DanglingSnapshots.Run();
}
