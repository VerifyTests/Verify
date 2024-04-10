namespace VerifyTests;

public partial class VerifySettings
{
    FirstVerify? handleOnFirstVerify;

    public void OnFirstVerify(FirstVerify firstVerify) =>
        handleOnFirstVerify += firstVerify;

    public void OnDelete(VerifyDelete verifyDelete) =>
        handleOnVerifyDelete += verifyDelete;

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

    internal async Task RunOnVerifyMismatch(FilePair item, string? message)
    {
        if (handleOnVerifyMismatch is not null)
        {
            await  handleOnVerifyMismatch(item, message);
        }

        await VerifierSettings.RunOnVerifyMismatch(item, message);
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