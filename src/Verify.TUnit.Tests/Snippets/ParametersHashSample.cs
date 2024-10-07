#region UseParametersHashTUnit

public class ParametersHashSample
{
    [Test]
    [Arguments("Value1")]
    [Arguments("Value2")]
    public Task HashParametersUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        settings.HashParameters();
        return Verify(arg, settings);
    }

    [Test]
    [Arguments("Value1")]
    [Arguments("Value2")]
    public Task HashParametersUsageFluent(string arg) =>
        Verify(arg)
            .HashParameters();

    [Test]
    [Arguments("Value1")]
    [Arguments("Value2")]
    public Task HashParametersOmitPassingParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.HashParameters();
        return Verify(arg, settings);
    }

    [Test]
    [Arguments("Value1")]
    [Arguments("Value2")]
    public Task HashParametersOmitPassingParametersFluent(string arg) =>
        Verify(arg)
            .HashParameters();
}

#endregion