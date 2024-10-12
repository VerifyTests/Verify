public class ParametersSample
{
    public static IEnumerable<object[]> GetDecimalData()
    {
        yield return
        [
            (decimal) 1.1
        ];
    }

    #region IgnoreParametersForVerifiedXunit

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParametersForVerified(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified(arg);
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedFluentXunit

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParametersForVerifiedFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified(arg);

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsXunit

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParametersForVerifiedCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified($"Number{arg}");
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsFluentXunit

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParametersForVerifiedCustomParamsFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified($"Number{arg}");

    #endregion

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        settings.IgnoreParameters(nameof(arg));
        return Verify("value", settings);
    }

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParametersFluent(string arg) =>
        Verify("value")
            .UseParameters(arg)
            .IgnoreParameters(nameof(arg));

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParametersCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters($"Number{arg}");
        settings.IgnoreParameters(nameof(arg));
        return Verify("value", settings);
    }

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParametersCustomParamsFluent(string arg) =>
        Verify("value")
            .UseParameters(arg)
            .IgnoreParameters(nameof(arg));

    [Theory]
    [MemberData(nameof(GetDecimalData))]
    public Task Decimal(decimal arg) =>
        Verify(arg)
            .UseParameters(arg);

    [Theory]
    [InlineData((float) 1.1)]
    public Task Float(float arg) =>
        Verify(arg)
            .UseParameters(arg);

    [Theory]
    [InlineData(1.1d)]
    public Task Double(double arg) =>
        Verify(arg)
            .UseParameters(arg);

    #region InlineDataXunit

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task InlineDataUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task InlineDataUsageFluent(string arg) =>
        Verify(arg)
            .UseParameters(arg);

    #endregion

    #region UseParametersXunit

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task UseParametersUsage(string arg)
    {
        var somethingToVerify = $"{arg} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg);
    }

    #endregion

    #region UseParametersSubSetXunit

    [Theory]
    [InlineData("Value1", "Value2", "Value3")]
    public Task UseParametersSubSet(string arg1, string arg2, string arg3)
    {
        var somethingToVerify = $"{arg1} {arg2} {arg3} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg1, arg2);
    }

    #endregion

    #region MemberDataXunit

    [Theory]
    [MemberData(nameof(GetData))]
    public Task MemberDataUsage(string arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetData))]
    public Task MemberDataUsageFluent(string arg) =>
        Verify(arg)
            .UseParameters(arg);

    public static IEnumerable<object[]> GetData()
    {
        yield return
        [
            "Value1"
        ];
        yield return
        [
            "Value2"
        ];
    }

    #endregion

    #region UseTextForParametersInstanceXunit

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task UseTextForParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(arg);
        return Verify(arg + "UseTextForParameters", settings);
    }

    #endregion

    #region UseTextForParametersFluentXunit

    [Theory]
    [InlineData("Value1")]
    [InlineData("Value2")]
    public Task UseTextForParametersFluent(string arg) =>
        Verify(arg + "UseTextForParametersFluent")
            .UseTextForParameters(arg);

    #endregion
}