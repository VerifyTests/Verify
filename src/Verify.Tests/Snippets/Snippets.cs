// ReSharper disable UnusedParameter.Local

public class Snippets
{
    // ReSharper disable once UnusedMember.Local
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

    // ReSharper disable once UnusedMember.Local
    void EnableClipboard()
    {
        #region EnableClipboard

        ClipboardAccept.Enable();

        #endregion
    }

    #region OnHandlers

    public Task OnHandlersSample()
    {
        VerifierSettings.OnVerify(
            before: () => Debug.WriteLine("before"),
            after: () => Debug.WriteLine("after"));
        VerifierSettings.OnFirstVerify(
            (receivedFile, receivedText) =>
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

    // ReSharper disable once UnusedMember.Local
    void TreatAsString()
    {
        #region TreatAsString

        VerifierSettings.TreatAsString<ClassWithToString>(
            (target, settings) => target.Property);

        #endregion
    }

    // ReSharper disable once UnusedMember.Local
    class ClassWithToString
    {
        public string Property { get; } = null!;
    }

    // ReSharper disable once UnusedMember.Local
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

    // ReSharper disable once UnusedMember.Local
    void AutoVerify()
    {
        #region AutoVerify

        var settings = new VerifySettings();
        settings.AutoVerify();

        #endregion
    }

    // ReSharper disable once UnusedMember.Local
    void DisableDiff()
    {
        #region DisableDiff

        var settings = new VerifySettings();
        settings.DisableDiff();

        #endregion
    }

    // ReSharper disable once UnusedMember.Local
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

    // ReSharper disable once UnusedMember.Local
    void Converter()
    {
        #region JsonConverter

        VerifierSettings.AddExtraSettings(
            _ => _.Converters.Add(new CompanyConverter()));

        #endregion
    }

    #region CompanyConverter

    class CompanyConverter :
        WriteOnlyJsonConverter<Company>
    {
        public override void Write(VerifyJsonWriter writer, Company company) =>
            writer.WriteMember(company, company.Name, "Name");
    }

    #endregion

    class Company
    {
        public string Name { get; } = null!;
    }

    // ReSharper disable once UnusedMember.Local
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