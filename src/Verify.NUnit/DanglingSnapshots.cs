public static class DanglingSnapshots
{
    public static void Run()
    {
        var state = TestExecutionContext.CurrentContext.CurrentResult.ResultState;
        if (state == ResultState.Success)
        {
            DanglingSnapshotsCheck.Run(true);
        }
    }
}