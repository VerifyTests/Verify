#region UseParametersHashXunitV3

public class ParametersHashSample
{
    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task HashParametersUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.HashParameters();
        return Verify(arg, settings);
    }

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task HashParametersUsageFluent(string arg) =>
        Verify(arg)
            .HashParameters();
}

#endregion