[TestClass]
public partial class ParametersSample
{
    #region MSTestDataRow

    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task DataRowUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task DataRowUsageFluent(string arg) =>
        Verify(arg)
            .UseParameters(arg);

    #endregion

    #region IgnoreParametersForVerifiedMsTest

    [DataTestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersForVerified(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified(arg);
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedFluentMsTest

    [DataTestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersForVerifiedFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified(arg);

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsMsTest

    [DataTestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersForVerifiedCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified($"Number{arg}");
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsFluentMsTest

    [DataTestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersForVerifiedFluentCustomParams(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified($"Number{arg}");

    #endregion
}