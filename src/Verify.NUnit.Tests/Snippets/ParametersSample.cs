using NUnit.Framework;
using VerifyNUnit;

[TestFixture]
public class ParametersSample
{
    [TestCase("1.1")]
    public async Task Decimal(decimal arg)
    {
        await Verifier.Verify(arg)
            .UseParameters(arg);
    }

    [TestCase((float) 1.1)]
    public async Task Float(float arg)
    {
        await Verifier.Verify(arg)
            .UseParameters(arg);
    }

    [TestCase(1.1d)]
    public async Task Double(double arg)
    {
        await Verifier.Verify(arg)
            .UseParameters(arg);
    }

    #region NUnitTestCase

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task TestCaseUsage(string arg)
    {
        return Verifier.Verify(arg);
    }

    #endregion
}