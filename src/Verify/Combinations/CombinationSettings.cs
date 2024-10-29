namespace VerifyTests;

public static class CombinationSettings
{
    public static bool CaptureExceptionsEnabled { get; private set; }

    public static void CaptureExceptions() =>
        CaptureExceptionsEnabled = true;

    static Func<Task>? before;

    internal static Task RunBeforeCallbacks()
    {
        if (before == null)
        {
            return Task.CompletedTask;
        }

        return before();
    }

    static Func<Task>? after;

    internal static Task RunAfterCallbacks()
    {
        if (after == null)
        {
            return Task.CompletedTask;
        }

        return after();
    }

    public static void UseCallbacks(Func<Task> before, Func<Task> after)
    {
        CombinationSettings.before += before;
        CombinationSettings.after += after;
    }
}