#region UseParametersHashFixie

public class ParametersHashSample
{
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
            .UseParameters(arg)
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