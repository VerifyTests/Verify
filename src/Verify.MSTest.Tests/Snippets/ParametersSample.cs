[TestClass]
public partial class ParametersSample
{
    #region UseTextForParametersMSTest

    [TestMethod]
    [DataRow("Value1")]
    public Task UseTextForParameters(string arg) =>
        Verify(arg)
            .UseTextForParameters("TextForParameter");

    #endregion

    #region DataRowInstanceMSTest

    [TestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task DataRowUsage(string arg) =>
        Verify(arg);

    #endregion

    #region IgnoreParametersForVerifiedMSTest

    [TestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersForVerified(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified(arg);
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedFluentMSTest

    [TestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersForVerifiedFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified(arg);

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsMSTest

    [TestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersForVerifiedCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified($"Number{arg}");
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsFluentMSTest

    [TestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersForVerifiedFluentCustomParams(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified($"Number{arg}");

    #endregion

    #region IgnoreParametersMSTest

    [TestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters(nameof(arg));
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersFluentMSTest

    [TestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersFluent(string arg) =>
        Verify("value")
            .IgnoreParameters(nameof(arg));

    #endregion

    #region IgnoreParametersCustomParamsMSTest

    [TestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters($"Number{arg}");
        settings.IgnoreParameters(nameof(arg));
        return Verify("value", settings);
    }

    #endregion

    #region UseParametersSubSetMSTest

    [TestMethod]
    [DataRow("Value1", "Value2", "Value3")]
    public Task UseParametersSubSet(string arg1, string arg2, string arg3)
    {
        var somethingToVerify = $"{arg1} {arg2} {arg3} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg1, arg2);
    }

    #endregion

    #region IgnoreParametersCustomParamsFluentMSTest

    [TestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersFluentCustomParams(string arg) =>
        Verify("value")
            .UseParameters($"Number{arg}")
            .IgnoreParameters(nameof(arg));

    #endregion
}