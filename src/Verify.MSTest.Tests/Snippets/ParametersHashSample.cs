namespace TheTests;

#region UseParametersHashMsTest

[TestClass]
public class ParametersHashSample :
    VerifyBase
{
    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task UseHashedParametersUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseHashedParameters(arg);
        return Verify(arg, settings);
    }

    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task UseHashedParametersUsageFluent(string arg) =>
        Verify(arg)
            .UseHashedParameters(arg);

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