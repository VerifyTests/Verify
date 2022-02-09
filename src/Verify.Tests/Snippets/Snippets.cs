using DiffEngine;
// ReSharper disable UnusedParameter.Local

public class Snippets
{
    #region OnHandlers

    public Task OnHandlersSample()
    {
        VerifierSettings.OnVerify(
            before: () => { Debug.WriteLine("before"); },
            after: () => { Debug.WriteLine("after"); });
        VerifierSettings.OnFirstVerify(
            receivedFile =>
            {
                Debug.WriteLine(receivedFile);
                return Task.CompletedTask;
            });
        VerifierSettings.OnVerifyMismatch(
            (filePair, message) =>
            {
                Debug.WriteLine(filePair.ReceivedPath);
                Debug.WriteLine(filePair.VerifiedPath);
                Debug.WriteLine(message);
                return Task.CompletedTask;
            });
        return Verify("value");
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
            _.TypeNameHandling = TypeNameHandling.All;
        });

        #endregion

        #region ExtraSettingsInstance

        var settings = new VerifySettings();
        settings.AddExtraSettings(_ =>
        {
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
        public override void Write(VerifyJsonWriter writer, Company company)
        {
            writer.WriteProperty(company, company.Name, "Name");
        }
    }

    #endregion

    class Company
    {
        public string Name { get; set; } = null!;
    }
}