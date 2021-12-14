[UsesVerify]
public class ParametersSample
{
    public static IEnumerable<object[]> GetDecimalData()
    {
        yield return new object[] {(decimal) 1.1};
    }

    [Theory]
    [MemberData(nameof(GetDecimalData))]
    public async Task Decimal(decimal arg)
    {
        await Verify(arg)
            .UseParameters(arg);
    }

    [Theory]
    [InlineData((float) 1.1)]
    public async Task Float(float arg)
    {
        await Verify(arg)
            .UseParameters(arg);
    }

    [Theory]
    [InlineData(1.1d)]
    public async Task Double(double arg)
    {
        await Verify(arg)
            .UseParameters(arg);
    }

    #region xunitInlineData

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
    public Task InlineDataUsageFluent(string arg)
    {
        return Verify(arg)
            .UseParameters(arg);
    }

    #endregion

    #region xunitMemberData

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
    public Task MemberDataUsageFluent(string arg)
    {
        return Verify(arg)
            .UseParameters(arg);
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"Value1"};
        yield return new object[] {"Value2"};
    }

    #endregion
}