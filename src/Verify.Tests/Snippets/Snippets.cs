using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Verify;
using VerifyXunit;
using Xunit.Abstractions;

public class Snippets:
    VerifyBase
{
    #region OnHandlers
    public async Task OnHandlersSample()
    {
        var settings = new VerifySettings();
        settings.OnFirstVerify(
            receivedFile =>
            {
                Debug.WriteLine(receivedFile);
                return Task.CompletedTask;
            });
        settings.OnVerifyMismatch(
            (receivedFile, verifiedFile, message) =>
            {
                Debug.WriteLine(receivedFile);
                Debug.WriteLine(verifiedFile);
                Debug.WriteLine(message);
                return Task.CompletedTask;
            });
        await Verify("value", settings);
    }
    #endregion

    void DisableClipboard()
    {
        #region DisableClipboard

        var settings = new VerifySettings();
        settings.DisableClipboard();

        #endregion
    }

    void EnableClipboard()
    {
        #region EnableClipboard

        var settings = new VerifySettings();
        settings.EnableClipboard();

        #endregion
    }

    void DisableClipboardGlobal()
    {
        #region DisableClipboardGlobal

        SharedVerifySettings.DisableClipboard();

        #endregion
    }

    void AutoVerify()
    {
        #region AutoVerify

        var settings = new VerifySettings();
        settings.AutoVerify();

        #endregion
    }

    void DisableDiff()
    {
        #region DisableDiff

        var settings = new VerifySettings();
        settings.DisableDiff();

        #endregion
    }

    void ApplyExtraSettingsSample()
    {
        #region ExtraSettings

        var settings = new VerifySettings();
        settings.AddExtraSettings(_ =>
        {
            _.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            _.TypeNameHandling = TypeNameHandling.All;
        });

        #endregion
    }

    public Snippets(ITestOutputHelper output) :
        base(output)
    {
    }
}