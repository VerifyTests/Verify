// ReSharper disable UnusedParameter.Local

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
public class Tests
{
    // ReSharper disable once UnusedMember.Local
    void DerivePathInfo() =>
    #region DerivePathInfoFixie

        Verifier.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) => new(
                directory: Path.Combine(projectDirectory, "Snapshots"),
                typeName: type.Name,
                methodName: method.Name));
    #endregion


    [TestCase("Value1")]
    public Task UseFileNameWithParam(string arg) =>
        Verify(arg)
            .UseFileName("UseFileNameWithParam");

    public Task StringTarget() =>
        Verify(new Target("txt", "Value"));

    #region ExplicitTargetsFixie

    public Task WithTargets() =>
        Verify(
            new
            {
                Property = "Value"
            },
            [
                new(
                    extension: "txt",
                    data: "Raw target value",
                    name: "targetName")
            ]);

    #endregion

    public Task EnumerableTargets() =>
        Verify(
        [
            new Target(
                extension: "txt",
                data: "Raw target value",
                name: "targetName")
        ]);

#if NET9_0
    static string directoryPathToVerify = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify");
    static string pathToArchive = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify.zip");

    #region VerifyDirectoryFixie

    public Task WithDirectory() =>
        VerifyDirectory(directoryPathToVerify);

    #endregion

    #region VerifyZipFixie

    public Task WithZip() =>
        VerifyZip(pathToArchive);

    #endregion

    public Task WithZipBytes() =>
        VerifyZip(File.ReadAllBytes(pathToArchive));

    #region VerifyZipWithStructureFixie

    public Task WithZipAndStructure() =>
        VerifyZip(pathToArchive, includeStructure: true);

    #endregion
    public Task WithZipAndPersistArchive() =>
        VerifyZip(pathToArchive, persistArchive: true);

#endif
}