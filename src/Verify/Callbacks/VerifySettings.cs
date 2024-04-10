namespace VerifyTests;

public partial class VerifySettings
{
    FirstVerify? handleOnFirstVerify;

    public void OnFirstVerify(FirstVerify firstVerify)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        handleOnFirstVerify += firstVerify;
    }

    public void OnDelete(VerifyDelete verifyDelete)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        handleOnVerifyDelete += verifyDelete;
    }

    VerifyDelete? handleOnVerifyDelete;

    internal async Task RunOnFirstVerify(NewResult item)
    {
        if (handleOnFirstVerify is not null)
        {
            await handleOnFirstVerify(item.File, item.ReceivedText?.ToString());
        }

        await VerifierSettings.RunOnFirstVerify(item);
    }

    VerifyMismatch? handleOnVerifyMismatch;

    internal async Task RunOnVerifyDelete(string file)
    {
        if (handleOnVerifyDelete is not null)
        {
            await handleOnVerifyDelete(file);
        }

        await VerifierSettings.RunOnVerifyDelete(file);
    }

    internal Task RunOnVerifyMismatch(FilePair item, string? message)
    {
        if (handleOnVerifyMismatch is null)
        {
            return Task.CompletedTask;
        }

        return handleOnVerifyMismatch(item, message);
    }

    public void OnVerifyMismatch(VerifyMismatch verifyMismatch)
    {
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        handleOnVerifyMismatch += verifyMismatch;
    }

    public void OnVerify(Action before, Action after)
    {
        beforeVerify += before;
        afterVerify += after;
    }

    Action? beforeVerify;

    internal void RunBeforeCallbacks()
    {
        beforeVerify?.Invoke();
        VerifierSettings.RunBeforeCallbacks();
    }

    Action? afterVerify;

    internal void RunAfterCallbacks()
    {
        afterVerify?.Invoke();
        VerifierSettings.RunAfterCallbacks();
    }
}