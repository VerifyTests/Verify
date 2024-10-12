public class ParametersHashSample
{
    #region UseParametersHashInstanceTUnit

    [Test]
    [Arguments("Value1")]
    [Arguments("Value2")]
    public Task HashParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.HashParameters();
        return Verify(arg, settings);
    }

    #endregion

    #region UseParametersHashFluentTUnit

    [Test]
    [Arguments("Value1")]
    [Arguments("Value2")]
    public Task HashParametersFluent(string arg) =>
        Verify(arg)
            .HashParameters();

    #endregion
}