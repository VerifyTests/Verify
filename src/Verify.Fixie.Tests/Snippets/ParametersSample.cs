// ReSharper disable UnusedParameter.Global
public class ParametersSample
{
    [TestCase("1.1")]
    public Task Decimal(decimal arg) =>
        Verify(arg)
            .UseParameters(arg);

    [TestCase((float) 1.1)]
    public Task Float(float arg) =>
        Verify(arg)
            .UseParameters(arg);

    [TestCase(1.1d)]
    public Task Double(double arg) =>
        Verify(arg)
            .UseParameters(arg);

    #region FixieTestCase

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
}