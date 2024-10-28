namespace VerifyTests;

public partial class VerifySettings
{
    FirstVerify? handleOnFirstVerify;

    public VerifySettings OnFirstVerify(FirstVerify firstVerify)
    {
        handleOnFirstVerify += firstVerify;
        return this;
    }

    public VerifySettings OnDelete(VerifyDelete verifyDelete)
    {
        handleOnVerifyDelete += verifyDelete;
        return this;
    }

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

    public VerifySettings OnVerifyMismatch(VerifyMismatch verifyMismatch)
    {
        handleOnVerifyMismatch += verifyMismatch;
        return this;
    }

    public VerifySettings OnVerify(Action before, Action after)
    {
        beforeVerify += before;
        afterVerify += after;
        return this;
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