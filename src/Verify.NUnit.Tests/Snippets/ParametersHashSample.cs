[TestFixture]
public class ParametersHashSample
{
    #region UseParametersHashInstanceNunit

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task HashParametersUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.HashParameters();
        return Verify(arg, settings);
    }

    #endregion

    #region UseParametersHashFluentNunit

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task HashParametersUsageFluent(string arg) =>
        Verify(arg)
            .HashParameters();

    #endregion
}