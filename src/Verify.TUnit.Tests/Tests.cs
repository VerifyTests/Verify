// ReSharper disable UnusedParameter.Local

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
public class Tests
{
    static void DerivePathInfo() =>
    #region DerivePathInfoNunit
        Verifier.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) => new(
                directory: Path.Combine(projectDirectory, "Snapshots"),
                typeName: type.Name,
                methodName: method.Name));
    #endregion

    [Arguments("Value1")]
    public Task UseFileNameWithParam(string arg) =>
        Verify(arg)
            .UseFileName("UseFileNameWithParam");

    [Test]
    [Arguments("Value1")]
    public Task UseTextForParameters(string arg) =>
        Verify(arg)
            .UseTextForParameters("TextForParameter");

    [Test]
    public Task StringTarget() =>
        Verify(new Target("txt", "Value"));

    #region ExplicitTargetsNunit

    [Test]
    public Task WithTargets() =>
        Verify(
            new
            {
                Property = "Value"
            },
            [
                new Target(
                    extension: "txt",
                    data: "Raw target value",
                    name: "targetName")
            ]);

    #endregion

    [Test]
    public Task EnumerableTargets() =>
        Verify(
        [
            new Target(
                    extension: "txt",
                    data: "Raw target value",
                    name: "targetName")
        ]);

    static string directoryPathToVerify = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify");
    static string pathToArchive = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify.zip");

    #region VerifyDirectoryNunit

    [Test]
    public Task WithDirectory() =>
        VerifyDirectory(directoryPathToVerify);

    #endregion

    #region VerifyZipNunit

    [Test]
    public Task WithZip() =>
        VerifyZip(pathToArchive);

    #endregion

    static List<TestAttachment> GetAttachments() =>
        TestExecutionContext.CurrentContext.CurrentResult.TestAttachments.ToList();

    [Test]
    public void ChangeHasAttachment()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        ThrowsAsync<VerifyException>(
            () => Verify("Bar", settings));
        var list = GetAttachments();
        AreEqual(1, list.Count);
        var file = Path.GetFileName(list[0].FilePath);
        AreEqual($"Tests.ChangeHasAttachment.{Namer.TargetFrameworkNameAndVersion}.received.txt", file);
    }

    [Test]
    public async Task AutoVerifyHasAttachment()
    {
        var path = CurrentFile.Relative("Tests.AutoVerifyHasAttachment.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        File.WriteAllText(fullPath,"Foo");
        var settings = new VerifySettings();
        settings.DisableDiff();
        settings.AutoVerify();
        await Verify("Bar", settings);
        var list = GetAttachments();
        AreEqual(1, list.Count);
        var file = Path.GetFileName(list[0].FilePath);
        AreEqual($"Tests.AutoVerifyHasAttachment.{Namer.TargetFrameworkNameAndVersion}.received.txt", file);
    }

    [Test]
    public void NewHasAttachment()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        ThrowsAsync<VerifyException>(
            () => Verify("Bar", settings));
        var list = GetAttachments();
        AreEqual(1, list.Count);
        var file = Path.GetFileName(list[0].FilePath);
        AreEqual($"Tests.NewHasAttachment.{Namer.TargetFrameworkNameAndVersion}.received.txt", file);
    }

    [Test]
    public void MultipleChangedHasAttachment()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        ThrowsAsync<VerifyException>(
            () => Verify("Bar", [new("txt", "Value")], settings));
        var list = GetAttachments();
        AreEqual(2, list.Count);
        var file0 = Path.GetFileName(list[0].FilePath);
        var file1 = Path.GetFileName(list[1].FilePath);
        AreEqual($"Tests.MultipleChangedHasAttachment.{Namer.TargetFrameworkNameAndVersion}#00.received.txt", file0);
        AreEqual($"Tests.MultipleChangedHasAttachment.{Namer.TargetFrameworkNameAndVersion}#01.received.txt", file1);
    }

    [Test]
    public void MultipleNewHasAttachment()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        ThrowsAsync<VerifyException>(
            () => Verify("Bar", [new("txt", "Value")], settings));
        var list = GetAttachments();
        AreEqual(2, list.Count);
        var file0 = Path.GetFileName(list[0].FilePath);
        var file1 = Path.GetFileName(list[1].FilePath);
        AreEqual($"Tests.MultipleNewHasAttachment.{Namer.TargetFrameworkNameAndVersion}#00.received.txt", file0);
        AreEqual($"Tests.MultipleNewHasAttachment.{Namer.TargetFrameworkNameAndVersion}#01.received.txt", file1);
    }
}