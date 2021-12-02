namespace TheTests;

#region MSTestDataRow

[TestClass]
public class ParametersSample :
    VerifyBase
{
    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task DataRowUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task DataRowUsageFluent(string arg)
    {
        return Verify(arg)
            .UseParameters(arg);
    }
}

#endregion