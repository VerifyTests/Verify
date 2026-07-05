namespace VerifyTests;

public partial class VerifySettings
{
    FirstVerify? handleOnFirstVerify;

    public void OnFirstVerify(FirstVerify firstVerify) =>
        handleOnFirstVerify += firstVerify;
    internal async Task RunOnFirstVerify(NewResult item, bool autoVerify)
    {
        if (handleOnFirstVerify is not null)
        {
            var receivedText = item.ReceivedText?.ToString();
            foreach (var handler in handleOnFirstVerify.GetInvocationList())
            {
                await ((FirstVerify) handler)(item.File, receivedText, autoVerify);
            }
        }

        await VerifierSettings.RunOnFirstVerify(item, autoVerify);
    }

    VerifyDelete? handleOnVerifyDelete;

    public void OnDelete(VerifyDelete verifyDelete) =>
        handleOnVerifyDelete += verifyDelete;


    internal async Task RunOnVerifyDelete(string file, bool autoVerify)
    {
        if (handleOnVerifyDelete is not null)
        {
            foreach (var handler in handleOnVerifyDelete.GetInvocationList())
            {
                await ((VerifyDelete) handler)(file, autoVerify);
            }
        }

        await VerifierSettings.RunOnVerifyDelete(file, autoVerify);
    }

    VerifyMismatch? handleOnVerifyMismatch;

    internal async Task RunOnVerifyMismatch(FilePair item, string? message, bool autoVerify)
    {
        if (handleOnVerifyMismatch is not null)
        {
            foreach (var handler in handleOnVerifyMismatch.GetInvocationList())
            {
                await ((VerifyMismatch) handler)(item, message, autoVerify);
            }
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