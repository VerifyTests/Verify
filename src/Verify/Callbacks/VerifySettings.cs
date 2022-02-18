namespace VerifyTests;

public static partial class VerifierSettings
{
    static FirstVerify? handleOnFirstVerify;

    public static void OnFirstVerify(FirstVerify firstVerify)
    {
        handleOnFirstVerify += firstVerify;
    }

    public static void OnDelete(VerifyDelete verifyDelete)
    {
        handleOnVerifyDelete += verifyDelete;
    }

    static VerifyDelete? handleOnVerifyDelete;

    internal static Task RunOnFirstVerify(NewResult item)
    {
        if (handleOnFirstVerify is null)
        {
            return Task.CompletedTask;
        }

        //TODO: pass in text
        return handleOnFirstVerify(item.File);
    }

    static VerifyMismatch? handleOnVerifyMismatch;

    internal static Task RunOnVerifyDelete(string file)
    {
        if (handleOnVerifyDelete is null)
        {
            return Task.CompletedTask;
        }

        return handleOnVerifyDelete(file);
    }

    internal static Task RunOnVerifyMismatch(FilePair item, string? message)
    {
        if (handleOnVerifyMismatch is null)
        {
            return Task.CompletedTask;
        }

        return handleOnVerifyMismatch(item, message);
    }

    public static void OnVerifyMismatch(VerifyMismatch verifyMismatch)
    {
        handleOnVerifyMismatch += verifyMismatch;
    }

    public static void OnVerify(Action before, Action after)
    {
        beforeVerify += before;
        afterVerify += after;
    }

    static Action? beforeVerify;

    internal static void RunBeforeCallbacks()
    {
        beforeVerify?.Invoke();
    }

    static Action? afterVerify;

    internal static void RunAfterCallbacks()
    {
        afterVerify?.Invoke();
    }
}