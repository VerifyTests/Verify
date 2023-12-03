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
}