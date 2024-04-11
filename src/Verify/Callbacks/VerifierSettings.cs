namespace VerifyTests;

public static partial class VerifierSettings
{
    static FirstVerify? handleOnFirstVerify;

    public static void OnFirstVerify(FirstVerify firstVerify)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        handleOnFirstVerify += firstVerify;
    }

    public static void OnDelete(VerifyDelete verifyDelete)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        handleOnVerifyDelete += verifyDelete;
    }

    static VerifyDelete? handleOnVerifyDelete;

    internal static Task RunOnFirstVerify(NewResult item, bool autoVerify)
    {
        if (handleOnFirstVerify is null)
        {
            return Task.CompletedTask;
        }

        return handleOnFirstVerify(item.File, item.ReceivedText?.ToString(), autoVerify);
    }

    static VerifyMismatch? handleOnVerifyMismatch;

    internal static Task RunOnVerifyDelete(string file, bool autoVerify)
    {
        if (handleOnVerifyDelete is null)
        {
            return Task.CompletedTask;
        }

        return handleOnVerifyDelete(file, autoVerify);
    }

    internal static Task RunOnVerifyMismatch(FilePair item, string? message, bool autoVerify)
    {
        if (handleOnVerifyMismatch is null)
        {
            return Task.CompletedTask;
        }

        return handleOnVerifyMismatch(item, message, autoVerify);
    }

    public static void OnVerifyMismatch(VerifyMismatch verifyMismatch)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        handleOnVerifyMismatch += verifyMismatch;
    }

    public static void OnVerify(Action before, Action after)
    {
        beforeVerify += before;
        afterVerify += after;
    }

    static Action? beforeVerify;

    internal static void RunBeforeCallbacks() =>
        beforeVerify?.Invoke();

    static Action? afterVerify;

    internal static void RunAfterCallbacks() =>
        afterVerify?.Invoke();
}