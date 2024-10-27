namespace VerifyTests;

public static class VerifyCombinationSettings
{
    public static bool CaptureExceptionsEnabled { get; private set; }

    public static void CaptureExceptions() =>
        CaptureExceptionsEnabled = true;

    static Func<Task>? beforeVerify;

    internal static void RunBeforeCallbacks() =>
        beforeVerify?.Invoke();

    static Func<Task>? afterVerify;

    internal static void RunAfterCallbacks() =>
        afterVerify?.Invoke();

    public static void UseCallbacks(Func<Task> before, Func<Task> after)
    {
        beforeVerify += before;
        afterVerify += after;
    }
}