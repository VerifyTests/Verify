﻿// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local

public class Snippets
{
    async Task ChangeDefaultsPerVerification(object target)
    {
        #region ChangeDefaultsPerVerification

        var settings = new VerifySettings();
        settings.DontIgnoreEmptyCollections();
        settings.DontScrubGuids();
        settings.DontScrubDateTimes();
        await Verify(target, settings);

        #endregion

        #region ChangeDefaultsPerVerification

        await Verify(target)
            .DontIgnoreEmptyCollections()
            .DontScrubGuids()
            .DontScrubDateTimes();

        #endregion
    }

    void TreatAsString() =>
    #region TreatAsString
        VerifierSettings.TreatAsString<ClassWithToString>(
            (target, settings) => target.Property);
    #endregion

    record ClassWithToString(string Property);

    void DerivePathInfoAppVeyor()
    {
        // ReSharper disable once ArrangeStaticMemberQualifier

        #region DerivePathInfoAppVeyor

        if (BuildServerDetector.Detected)
        {
            var buildDirectory = Environment.GetEnvironmentVariable("APPVEYOR_BUILD_FOLDER")!;
            DerivePathInfo(
                (sourceFile, projectDirectory, typeName, methodName) =>
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

    void AutoVerifyDelegate()
    {
        #region AutoVerifyDelegate

        var settings = new VerifySettings();
        settings.AutoVerify(
            verifiedFile =>
                Path.GetExtension(verifiedFile) == "png");

        #endregion
    }

    #region AutoVerifyFluent

    [Fact]
    public Task AutoVerifyFluent() =>
        Verify("Value")
            .AutoVerify();

    #endregion


    #region AutoVerifyIncludeBuildServer

    [Fact]
    public Task AutoVerifyIncludeBuildServer() =>
        Verify("Value")
            .AutoVerify(includeBuildServer: false);

    #endregion

    public Task OverrideTreatAsStringDefaults()
    {
    #region OverrideTreatAsStringDefaults
        VerifierSettings.TreatAsString<DateTime>(
            (target, settings) => target.ToString("D"));
        #endregion
        return Verify(DateTime.Now);
    }

    #region AutoVerifyFluentDelegate

    [Fact]
    public Task AutoVerifyFluentDelegate() =>
        Verify("Value")
            .AutoVerify(
                verifiedFile =>
                    Path.GetExtension(verifiedFile) == "png");

    #endregion

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

        VerifierSettings.AddExtraSettings(
            _ => _.TypeNameHandling = TypeNameHandling.All);

        #endregion

        #region ExtraSettingsInstance

        var settings = new VerifySettings();
        settings.AddExtraSettings(
            _ => _.TypeNameHandling = TypeNameHandling.All);

        #endregion
    }


    async Task VerifyFuncOfTaskOfT()
    {
        var repo = new Repo();
        var id = 1;

        #region VerifyFuncOfTaskOfT

        await Verify(
            async () => new
            {
                Foo = await repo.GetFoo(id),
                Bars = await repo.GetBars(id)
            });

        #endregion
    }

    class Repo
    {
        // ReSharper disable once MemberCanBeMadeStatic.Local
        public Task<object> GetFoo(int id) => throw new NotImplementedException();

        // ReSharper disable once MemberCanBeMadeStatic.Local
        public Task<object> GetBars(int id) => throw new NotImplementedException();
    }
}