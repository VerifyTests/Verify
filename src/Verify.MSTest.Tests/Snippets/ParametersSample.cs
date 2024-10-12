[TestClass]
public partial class ParametersSample
{
    #region DataRowMSTest

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

    #region IgnoreParametersMsTest

    [DataTestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        settings.IgnoreParameters(nameof(arg));
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersFluentMsTest

    [DataTestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersFluent(string arg) =>
        Verify("value")
            .UseParameters(arg)
            .IgnoreParameters(nameof(arg));

    #endregion

    #region IgnoreParametersCustomParamsMsTest

    [DataTestMethod]
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
    #region UseParametersMsTest

    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task UseParametersUsage(string arg)
    {
        var somethingToVerify = $"{arg} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg);
    }

    #endregion

    #region UseParametersSubSetMsTest

    [DataTestMethod]
    [DataRow("Value1", "Value2", "Value3")]
    public Task UseParametersSubSet(string arg1, string arg2, string arg3)
    {
        var somethingToVerify = $"{arg1} {arg2} {arg3} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg1, arg2);
    }

    #endregion
    #region IgnoreParametersCustomParamsFluentMsTest

    [DataTestMethod]
    [DataRow("One")]
    [DataRow("Two")]
    public Task IgnoreParametersFluentCustomParams(string arg) =>
        Verify("value")
            .UseParameters($"Number{arg}")
            .IgnoreParameters(nameof(arg));

    #endregion
}