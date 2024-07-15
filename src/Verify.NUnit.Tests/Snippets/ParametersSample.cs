[TestFixture]
public class ParametersSample
{
    [TestCase("1.1")]
    public Task Decimal(decimal arg) =>
        Verify(arg);

    [TestCase((float) 1.1)]
    public Task Float(float arg) =>
        Verify(arg);

    [TestCase(1.1d)]
    public Task Double(double arg) =>
        Verify(arg);

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerified(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified(arg);
        return Verify("value", settings);
    }

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerifiedFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified(arg);

    #region NUnitTestCase

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task TestCaseUsage(string arg) =>
        Verify(arg);

    #endregion

    [TestCase("Value2")]
    public Task SuppliedDoesNotMatchArg(string arg) =>
        Verify("Foo")
            .UseParameters("notTheArg");

    // #region nunitAutoFixture
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