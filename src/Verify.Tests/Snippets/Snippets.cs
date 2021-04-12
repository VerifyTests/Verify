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

    public Task OnHandlersSample()
    {
        VerifierSettings.OnFirstVerify(
            receivedFile =>
            {
                Debug.WriteLine(receivedFile);
                return Task.CompletedTask;
            });
        VerifierSettings.OnVerifyMismatch(
            (filePair, message) =>
            {
                Debug.WriteLine(filePair.Received);
                Debug.WriteLine(filePair.Verified);
                Debug.WriteLine(message);
                return Task.CompletedTask;
            });
        return Verifier.Verify("value");
    }

    #endregion

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

    void DisableClipboardGlobal()
    {
        #region DisableClipboardGlobal

        VerifierSettings.DisableClipboard();

        #endregion
    }

    void DerivePathInfo()
    {
        #region DerivePathInfo

        VerifierSettings.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
            {
                return new(
                    directory: Path.Combine(projectDirectory, "Snapshots"),
                    typeName: type.Name,
                    methodName: method.Name);
            });

        #endregion
    }

    void DerivePathInfoAppVeyor()
    {
        #region DerivePathInfoAppVeyor

        if (BuildServerDetector.Detected)
        {
            var buildDirectory = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER")!;
            VerifierSettings.DerivePathInfo(
                (sourceFile, projectDirectory, type, method) =>
                {
                    var testDirectory = Path.GetDirectoryName(sourceFile)!;
                    var testDirectorySuffix = testDirectory.Replace(projectDirectory, string.Empty);
                    return new(directory: Path.Combine(buildDirectory, testDirectorySuffix));
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

        VerifierSettings.AddExtraSettings(
            _ =>
            {
                _.Converters.Add(new CompanyConverter());
            });

        #endregion
    }

    #region CompanyConverter

    class CompanyConverter :
        WriteOnlyJsonConverter<Company>
    {
        public override void WriteJson(
            JsonWriter writer,
            Company company,
            JsonSerializer serializer,
            IReadOnlyDictionary<string, object> context)
        {
            serializer.Serialize(writer, company.Name);
        }
    }

    #endregion

    class Company
    {
        public string Name { get; set; } = null!;
    }
}