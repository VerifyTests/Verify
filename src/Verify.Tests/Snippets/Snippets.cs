using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DiffEngine;
using Newtonsoft.Json;
using VerifyTests;
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
            (filePair, message) =>
            {
                Debug.WriteLine(filePair.Received);
                Debug.WriteLine(filePair.Verified);
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

    void TreatAsString()
    {
        #region TreatAsString

        VerifierSettings.TreatAsString<ClassWithToString>(
            (target, settings) => target.Property);

        #endregion
    }

    class ClassWithToString
    {
        public string Property { get; set; } = null!;
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
                    var testDirectorySuffix = testDirectory.Replace(projectDirectory, string.Empty);
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
        #region ExtraSettingsGlobal

        VerifierSettings.AddExtraSettings(_ =>
        {
            _.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            _.TypeNameHandling = TypeNameHandling.All;
        });

        #endregion

        #region ExtraSettingsInstance

        var settings = new VerifySettings();
        settings.AddExtraSettings(_ =>
        {
            _.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
            _.TypeNameHandling = TypeNameHandling.All;
        });

        #endregion
    }

    void Converter()
    {
        #region JsonConverter

        VerifierSettings.AddExtraSettings(_ => { _.Converters.Add(new CompanyConverter()); });

        #endregion
    }

    #region CompanyConverter

    class CompanyConverter :
        WriteOnlyJsonConverter<Company>
    {
        public override void WriteJson(
            JsonWriter writer,
            Company? company,
            JsonSerializer serializer,
            IReadOnlyDictionary<string, object> context)
        {
            if (company != null)
            {
                serializer.Serialize(writer, company.Name);
            }
        }
    }

    #endregion

    class Company
    {
        public string Name { get; set; } = null!;
    }
}