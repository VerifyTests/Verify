namespace TheTests;

#region UseParametersHashNunit

[TestFixture]
public class ParametersHashSample
{
    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task UseHashedParametersUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseHashedParameters(arg);
        return Verify(arg, settings);
    }

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task UseHashedParametersUsageFluent(string arg) =>
        Verify(arg)
            .UseHashedParameters(arg);

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task HashParametersUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        settings.HashParameters();
        return Verify(arg, settings);
    }

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task HashParametersUsageFluent(string arg) =>
        Verify(arg)
            .HashParameters();

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task HashParametersOmitPassingParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.HashParameters();
        return Verify(arg, settings);
    }

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task HashParametersOmitPassingParametersFluent(string arg) =>
        Verify(arg)
            .HashParameters();
}

#endregion