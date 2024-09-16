public class ParametersSample
{
    [Test]
    [Arguments(1.1f)]
    public Task Float(float arg) =>
        Verify(arg);

    [Test]
    [Arguments(1.1d)]
    public Task Double(double arg) =>
        Verify(arg);

    #region IgnoreParametersForVerifiedTunit

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

    #region IgnoreParametersForVerifiedFluentTunit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParametersForVerifiedFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified();

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsTunit

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

    #region IgnoreParametersForVerifiedCustomParamsFluentTunit

    [Test]
    [Arguments("One")]
    [Arguments("Two")]
    public Task IgnoreParametersForVerifiedCustomParamsFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified($"Number{arg}");

    #endregion

    #region TUnitTestCase

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

    // #region TUnitAutoFixture
    //
    // [Theory]
    // [InlineAutoData(42)]
    // public Task AutoFixtureUsage(int stable, string random1, string random2)
    // {
    //     var result = MethodBeingTested(stable, random1, random2);
    //     return Verify(result)
    //         .UseParameters(stable);
    // }
    //
    // #endregion
    //
    // // ReSharper disable UnusedParameter.Local
    // static int MethodBeingTested(int stable, string random1, string random2) =>
    //     stable;
}