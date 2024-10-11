// ReSharper disable UnusedParameter.Global
public class ParametersSample
{
    [TestCase("1.1")]
    public Task Decimal(decimal arg) =>
        Verify(arg)
            .UseParameters(arg);

    [TestCase((float)1.1)]
    public Task Float(float arg) =>
        Verify(arg)
            .UseParameters(arg);

    [TestCase(1.1d)]
    public Task Double(double arg) =>
        Verify(arg)
            .UseParameters(arg);

    #region UseParametersFixie

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task UseParametersUsage(string arg)
    {
        var somethingToVerify = $"{arg} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg);
    }

    #endregion

    #region UseTextForParametersFixie

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task UseTextForParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(arg);
        return Verify(arg + "UseTextForParameters", settings);
    }

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task UseTextForParametersFluent(string arg) =>
        Verify(arg + "UseTextForParametersFluent")
            .UseTextForParameters(arg);

    #endregion

    #region UseParametersSubSetFixie

    [TestCase("Value1", "Value2", "Value3")]
    public Task UseParametersSubSet(string arg1, string arg2, string arg3)
    {
        var somethingToVerify = $"{arg1} {arg2} {arg3} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg1, arg2);
    }

    #endregion

    #region TestCaseFixie

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task TestCaseUsage(string arg) =>
        Verify(arg);

    #endregion

    [TestCase("Value2")]
    public Task SuppliedDoesNotMatchArg(string arg) =>
        Verify("Foo")
            .UseParameters("notTheArg");

    #region IgnoreParametersForVerifiedFixie

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerified(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified(arg);
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedFluentFixie

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerifiedFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified(arg);

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsFixie

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerifiedCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified($"Number{arg}");
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsFluentFixie

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerifiedFluentCustomParams(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified($"Number{arg}");

    #endregion

    #region IgnoreParametersFixie

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        settings.IgnoreParameters(nameof(arg));
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersFluentFixie

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersFluent(string arg) =>
        Verify("value")
            .UseParameters(arg)
            .IgnoreParameters(arg);

    #endregion

    #region IgnoreParametersCustomParamsFixie

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        settings.IgnoreParameters(nameof(arg));
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersCustomParamsFluentFixie

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersFluentCustomParams(string arg) =>
        Verify("value")
            .UseParameters(arg)
            .IgnoreParameters(nameof(arg));

    #endregion
}