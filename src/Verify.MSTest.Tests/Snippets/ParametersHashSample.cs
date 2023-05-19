namespace TheTests;

#region UseParametersHash

[TestClass]
public class ParametersHashSample :
    VerifyBase
{
    [DataTestMethod]
    [DataRow("Value1")]
    [DataRow("Value2")]
    public Task ParametersHashUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParametersHash(arg);
        return Verify(arg, settings);
    }
}

#endregion