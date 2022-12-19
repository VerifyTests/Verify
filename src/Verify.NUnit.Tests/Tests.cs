[TestFixture]
public class Tests
{
    [TestCase("Value1")]
    public Task UseFileNameWithParam(string arg) =>
        Verify(arg)
            .UseFileName("UseFileNameWithParam");

    [TestCase("Value1")]
    public Task UseTextForParameters(string arg) =>
        Verify(arg)
            .UseTextForParameters("TextForParameter");

    [TestCase("Value1", TestName = "CustomName")]
    public Task TestCaseWithName(string arg) =>
        Verify(arg)
            .UseTextForParameters("TextForParameter");

    [TestCase("Value1", TestName = "Custom>Name")]
    public Task TestCaseWithNameAndInvalidChars(string arg) =>
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
            new[]
            {
                new Target(
                    extension: "txt",
                    data: "Raw target value",
                    name: "targetName")
            });

    #endregion

    [Test]
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

    #region VerifyDirectoryNunit

    [Test]
    public Task WithDirectory() =>
        VerifyDirectory(directoryPathToVerify);

    #endregion
}