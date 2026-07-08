namespace VerifyTests;

public static partial class VerifierSettings
{
    static Func<string,Task>? addTestAttachment;

    internal static Task RunAddTestAttachment(string path)
    {
        if (addTestAttachment is null ||
            !addAttachments ||
            !BuildServerDetector.Detected)
        {
            return Task.CompletedTask;
        }

        return addTestAttachment(path);
    }

    internal static void AddTestAttachment(Func<string, Task> action)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        if (addTestAttachment != null)
        {
            throw new("AddTestAttachment already defined");
        }

        addTestAttachment = action;
    }

    static FirstVerify? handleOnFirstVerify;

    public static void OnFirstVerify(FirstVerify firstVerify)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        handleOnFirstVerify += firstVerify;
    }

    internal static async Task RunOnFirstVerify(NewResult item, bool autoVerify)
    {
        if (handleOnFirstVerify is null)
        {
            return;
        }

        var receivedText = item.ReceivedText?.ToString();
        // Await every registered handler; invoking the multicast delegate
        // directly would only await the last handler's Task.
        foreach (var handler in handleOnFirstVerify.GetInvocationList())
        {
            await ((FirstVerify) handler)(item.File, receivedText, autoVerify);
        }
    }

    static VerifyDelete? handleOnVerifyDelete;

    public static void OnDelete(VerifyDelete verifyDelete)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        handleOnVerifyDelete += verifyDelete;
    }

    static VerifyMismatch? handleOnVerifyMismatch;

    internal static async Task RunOnVerifyDelete(string file, bool autoVerify)
    {
        if (handleOnVerifyDelete is null)
        {
            return;
        }

        foreach (var handler in handleOnVerifyDelete.GetInvocationList())
        {
            await ((VerifyDelete) handler)(file, autoVerify);
        }
    }

    internal static async Task RunOnVerifyMismatch(FilePair item, string? message, bool autoVerify)
    {
        if (handleOnVerifyMismatch is null)
        {
            return;
        }

        foreach (var handler in handleOnVerifyMismatch.GetInvocationList())
        {
            await ((VerifyMismatch) handler)(item, message, autoVerify);
        }
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