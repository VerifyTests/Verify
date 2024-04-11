namespace VerifyTests;

public partial class VerifySettings
{
    FirstVerify? handleOnFirstVerify;

    public void OnFirstVerify(FirstVerify firstVerify) =>
        handleOnFirstVerify += firstVerify;

    public void OnDelete(VerifyDelete verifyDelete) =>
        handleOnVerifyDelete += verifyDelete;

    VerifyDelete? handleOnVerifyDelete;

    internal async Task RunOnFirstVerify(NewResult item, bool autoVerify)
    {
        if (handleOnFirstVerify is not null)
        {
            await handleOnFirstVerify(item.File, item.ReceivedText?.ToString(), autoVerify);
        }

        await VerifierSettings.RunOnFirstVerify(item, autoVerify);
    }

    VerifyMismatch? handleOnVerifyMismatch;

    internal async Task RunOnVerifyDelete(string file, bool autoVerify)
    {
        if (handleOnVerifyDelete is not null)
        {
            await handleOnVerifyDelete(file, autoVerify);
        }

        await VerifierSettings.RunOnVerifyDelete(file, autoVerify);
    }

    internal async Task RunOnVerifyMismatch(FilePair item, string? message, bool autoVerify)
    {
        if (handleOnVerifyMismatch is not null)
        {
            await handleOnVerifyMismatch(item, message, autoVerify);
        }

        await VerifierSettings.RunOnVerifyMismatch(item, message, autoVerify);
    }

    public void OnVerifyMismatch(VerifyMismatch verifyMismatch) =>
        handleOnVerifyMismatch += verifyMismatch;

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