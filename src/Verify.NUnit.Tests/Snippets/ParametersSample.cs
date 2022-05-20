using AutoFixture.NUnit3;

[TestFixture]
public class ParametersSample
{
    [TestCase("1.1")]
    public async Task Decimal(decimal arg) =>
        await Verify(arg)
            .UseParameters(arg);

    [TestCase((float) 1.1)]
    public async Task Float(float arg) =>
        await Verify(arg)
            .UseParameters(arg);

    [TestCase(1.1d)]
    public async Task Double(double arg) =>
        await Verify(arg)
            .UseParameters(arg);

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

    #region nunitAutoFixture

    [Theory]
    [InlineAutoData(42)]
    public Task AutoFixtureUsage(int stable, string random1, string random2)
    {
        var result = MethodBeingTested(stable, random1, random2);
        return Verify(result)
            .UseParameters(stable);
    }

    #endregion

    static int MethodBeingTested(int stable, string random1, string random2) =>
        stable;
}