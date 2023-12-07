// ReSharper disable UnusedParameter.Local

[TestClass]
public class Tests :
    VerifyBase
{
    // ReSharper disable once UnusedMember.Local
    void DerivePathInfo()
    {
        #region DerivePathInfoMSTest

        DerivePathInfo(
            (sourceFile, projectDirectory, type, method) => new(
                directory: Path.Combine(projectDirectory, "Snapshots"),
                typeName: type.Name,
                methodName: method.Name));

        #endregion
    }

    [DataTestMethod]
    [DataRow("Value1")]
    public Task MissingParameter(string arg) =>
        Verify("Foo");

    [DataTestMethod]
    [DataRow("Value1")]
    public Task UseFileNameWithParam(string arg) =>
        Verify(arg)
            .UseFileName("UseFileNameWithParam");

    [DataTestMethod]
    [DataRow("Value1")]
    public Task UseTextForParameters(string arg) =>
        Verify(arg)
            .UseTextForParameters("TextForParameter");

    [TestMethod]
    public Task StringTarget() =>
        Verify(new Target("txt", "Value"));

    #region ExplicitTargetsMsTest

    [TestMethod]
    public Task WithTargets() =>
        Verify(
            target: new
            {
                Property = "Value"
            },
            rawTargets:
            [
                new(
                    extension: "txt",
                    data: "Raw target value",
                    name: "targetName")
            ]);

    #endregion

    [TestMethod]
    public Task EnumerableTargets() =>
        Verify(
            new[]
            {
                new Target(
                    extension: "txt",
                    data: "Raw target value",
                    name: "targetName")
            });

    static string directoryPathToVerify = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify");

    #region VerifyDirectoryMsTest

    [TestMethod]
    public Task WithDirectory() =>
        VerifyDirectory(directoryPathToVerify);

    #endregion

    static string zipPath = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify.zip");

    #region WithZipMsTest

    [TestMethod]
    public Task WithZip() =>
        VerifyZip(zipPath);

    #endregion
}