#region UseParametersHashMsTest

[TestClass]
public partial class ParametersHashSample
{
    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task HashParametersUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        settings.HashParameters();
        return Verify(arg, settings);
    }

    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task HashParametersUsageFluent(string arg) =>
        Verify(arg)
            .UseParameters(arg)
            .HashParameters();
}

#endregion