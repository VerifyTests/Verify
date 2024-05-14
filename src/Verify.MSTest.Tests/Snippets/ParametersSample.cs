namespace TheTests;

#region MSTestDataRow

[TestClass]
[UsesVerify]
public partial class ParametersSample
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
    public Task DataRowUsageFluent(string arg) =>
        Verify(arg)
            .UseParameters(arg);
}

#endregion