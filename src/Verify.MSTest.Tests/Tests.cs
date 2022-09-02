// remove namespace when https://github.com/microsoft/testfx/issues/889

namespace TheTests;

[TestClass]
public class Tests :
    VerifyBase
{
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

    #region ExplicitTargetsMsTest

    [TestMethod]
    public Task WithTargets() =>
        Verify(
            target: new
            {
                Property = "Value"
            },
            rawTargets: new[]
            {
                new Target(
                    extension: "txt",
                    stringData: "Raw target value",
                    name: "targetName")
            });

    #endregion
}