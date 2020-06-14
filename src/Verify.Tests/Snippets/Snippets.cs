using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DiffEngine;
using Newtonsoft.Json;
using VerifyTesting;
using VerifyXunit;

public class Snippets
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
        await Verifier.Verify("value", settings);
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

        VerifierSettings.DisableClipboard();

        #endregion
    }

    void DeriveTestDirectory()
    {
        #region DeriveTestDirectory

        if (BuildServerDetector.Detected)
        {
            var buildDirectory = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER")!;
            VerifierSettings.DeriveTestDirectory(
                (sourceFile, projectDirectory) =>
                {
                    var testDirectory = Path.GetDirectoryName(sourceFile)!;
                    var testDirectorySuffix = testDirectory.Replace(projectDirectory!, string.Empty);
                    return Path.Combine(buildDirectory, testDirectorySuffix);
                });
        }

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
}