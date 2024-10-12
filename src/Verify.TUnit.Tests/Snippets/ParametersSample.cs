public class ParametersSample
{
    [Test]
    [Arguments(1.1F)]
    public Task Float(float arg) =>
        Verify(arg);

    [Test]
    [Arguments(1.1d)]
    public Task Double(double arg) =>
        Verify(arg);

    #region UseParametersTUnit

    [Test]
    [Arguments("Value1")]
    public Task UseParametersUsage(string arg)
    {
        var somethingToVerify = $"{arg} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg);
    }

    #endregion

    #region UseParametersSubSetTUnit

    [Test]
    [Arguments("Value1", "Value2", "Value3")]
    public Task UseParametersSubSet(string arg1, string arg2, string arg3)
    {
        var somethingToVerify = $"{arg1} {arg2} {arg3} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg1, arg2);
    }

    #endregion

    #region IgnoreParametersForVerifiedTUnit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParametersForVerified(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified();
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedFluentTUnit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParametersForVerifiedFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified();

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsTUnit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParametersForVerifiedCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified($"Number{arg}");
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsFluentTUnit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParametersForVerifiedCustomParamsFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified($"Number{arg}");

    #endregion

    #region IgnoreParametersTUnit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters();
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersFluentTUnit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParametersFluent(string arg) =>
        Verify("value")
            .IgnoreParameters();

    #endregion

    #region IgnoreParametersCustomParamsTUnit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParametersCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters(nameof(arg));
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersCustomParamsFluentTUnit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParametersCustomParamsFluent(string arg) =>
        Verify("value")
            .IgnoreParameters(nameof(arg));

    #endregion

    #region TestCaseTUnit

    [Test]
    [Arguments("Value1")]
    [Arguments("Value2")]
    public Task TestCaseUsage(string arg) =>
        Verify(arg);

    #endregion

    [Test]
    [Arguments("Value2")]
    public Task SuppliedDoesNotMatchArg(string arg) =>
        Verify("Foo")
            .UseParameters("notTheArg");

    #region UseTextForParametersInstanceTUnit

    [Test]
    [Arguments("Value1")]
    [Arguments("Value2")]
    public Task UseTextForParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(arg);
        return Verify(arg + "UseTextForParameters", settings);
    }

    #endregion

    #region UseTextForParametersFluentTUnit

    [Test]
    [Arguments("Value1")]
    [Arguments("Value2")]
    public Task UseTextForParametersFluent(string arg) =>
        Verify(arg + "UseTextForParametersFluent")
            .UseTextForParameters(arg);

    #endregion
}