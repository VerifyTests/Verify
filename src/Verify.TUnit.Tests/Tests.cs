// ReSharper disable UnusedParameter.Local

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

using VerifyTUnit;

public class Tests
{
    static void DerivePathInfo() =>
    #region DerivePathInfoTunit
        Verifier.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) => new(
                directory: Path.Combine(projectDirectory, "Snapshots"),
                typeName: type.Name,
                methodName: method.Name));
    #endregion

    [Test]
    [Arguments("Value1")]
    public Task UseFileNameWithParam(string arg) =>
        Verify(arg)
            .UseFileName("UseFileNameWithParam");

    [Test]
    public Task StringTarget() =>
        Verify(new Target("txt", "Value"));

    #region ExplicitTargetsTUnit

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

    #region VerifyDirectoryTunit

    [Test]
    public Task WithDirectory() =>
        VerifyDirectory(directoryPathToVerify);

    #endregion

    #region VerifyZipTunit

    [Test]
    public Task WithZip() =>
        VerifyZip(pathToArchive);

    #endregion

    [Test]
    public Task WithZipBytes() =>
        VerifyZip(File.ReadAllBytes(pathToArchive));

    #region VerifyZipWithStructureTunit

    [Test]
    public Task WithZipAndStructure() =>
        VerifyZip(pathToArchive, includeStructure: true);

    #endregion

    static IReadOnlyList<Artifact> GetAttachments() => TestContext.Current!.Artifacts;

    [Test]
    public async Task ChangeHasAttachment()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        await Assert.ThrowsAsync(
            () => Verify("Bar", settings));
        var list = GetAttachments();
        await Assert.That(list.Count).IsEqualTo(1);
        var expected = $"Tests.ChangeHasAttachment.{Namer.TargetFrameworkNameAndVersion}.received.txt";
        await Assert.That(list[0].File.Name).IsEqualTo(expected);
    }

#if NET9_0

    [Test]
    public async Task AutoVerifyHasAttachment()
    {
        var path = CurrentFile.Relative("Tests.AutoVerifyHasAttachment.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await File.WriteAllTextAsync(fullPath,"Foo");
        var settings = new VerifySettings();
        settings.DisableDiff();
        settings.AutoVerify();
        await Verify("Bar", settings);
        var list = GetAttachments();
        await Assert.That(list.Count).IsEqualTo(1);
        await Assert.That(list[0].File.Name)
            .IsEqualTo($"Tests.AutoVerifyHasAttachment.{Namer.TargetFrameworkNameAndVersion}.received.txt");
    }

#endif

    [Test]
    public async Task NewHasAttachment()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        await Assert.ThrowsAsync(
            () => Verify("Bar", settings));
        var list = GetAttachments();
        await Assert.That(list.Count).IsEqualTo(1);
        await Assert.That(list[0].File.Name)
            .IsEqualTo($"Tests.NewHasAttachment.{Namer.TargetFrameworkNameAndVersion}.received.txt");
    }

    [Test]
    public async Task MultipleChangedHasAttachment()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        await Assert.ThrowsAsync(
            () => Verify("Bar", [new("txt", "Value")], settings));
        var list = GetAttachments();
        await Assert.That(list.Count).IsEqualTo(2);
        await Assert.That(list[0].File.Name)
            .IsEqualTo($"Tests.MultipleChangedHasAttachment.{Namer.TargetFrameworkNameAndVersion}#00.received.txt");
        await Assert.That(list[1].File.Name)
            .IsEqualTo($"Tests.MultipleChangedHasAttachment.{Namer.TargetFrameworkNameAndVersion}#01.received.txt");
    }

    [Test]
    public async Task MultipleNewHasAttachment()
    {
        var settings = new VerifySettings();
        settings.DisableDiff();
        await Assert.ThrowsAsync(
            () => Verify("Bar", [new("txt", "Value")], settings));
        var list = GetAttachments();
        await Assert.That(list.Count).IsEqualTo(2);
        await Assert.That(list[0].File.Name)
            .IsEqualTo($"Tests.MultipleNewHasAttachment.{Namer.TargetFrameworkNameAndVersion}#00.received.txt");
        await Assert.That(list[0].File.Name)
            .IsEqualTo($"Tests.MultipleNewHasAttachment.{Namer.TargetFrameworkNameAndVersion}#00.received.txt");
    }
}