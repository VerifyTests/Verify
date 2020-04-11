using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DiffEngine;
using Newtonsoft.Json;
using Verify;
using VerifyXunit;
using Xunit.Abstractions;

public class Snippets:
    VerifyBase
{
    void AddCustomTool(string diffToolPath)
    {
        #region AddCustomTool
        DiffTools.AddCustomTool(
            supportsAutoRefresh: true,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
            exePath: diffToolPath,
            binaryExtensions: new[] {"jpg"});
        #endregion
    }

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
            (receivedFile, verifiedFile) =>
            {
                Debug.WriteLine(receivedFile);
                Debug.WriteLine(verifiedFile);
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