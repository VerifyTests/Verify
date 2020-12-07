using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DiffEngine;
using Newtonsoft.Json;
using VerifyTests;
using VerifyXunit;
// ReSharper disable UnusedParameter.Local

public class Snippets
{
    #region OnHandlers

    public async Task OnHandlersSample()
    {
        VerifySettings settings = new();
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

        VerifySettings settings = new();
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

        VerifySettings settings = new();
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

        VerifierSettings.DeriveTestDirectory(
            (sourceFile, projectDirectory) =>
            {
                var snapshotsDirectory = Path.Combine(projectDirectory, "Snapshots");
                Directory.CreateDirectory(snapshotsDirectory);
                return Path.Combine(snapshotsDirectory);
            });

        #endregion
    }

    void DeriveTestDirectoryAppVeyor()
    {
        #region DeriveTestDirectoryAppVeyor

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

        VerifySettings settings = new();
        settings.AutoVerify();

        #endregion
    }

    void DisableDiff()
    {
        #region DisableDiff

        VerifySettings settings = new();
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

        VerifySettings settings = new();
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