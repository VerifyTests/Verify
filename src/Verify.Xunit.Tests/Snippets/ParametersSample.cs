﻿using AutoFixture.Xunit2;

[UsesVerify]
public class ParametersSample
{
    public static IEnumerable<object[]> GetDecimalData()
    {
        yield return new object[]
        {
            (decimal) 1.1
        };
    }

    [Theory]
    [MemberData(nameof(GetDecimalData))]
    public async Task Decimal(decimal arg) =>
        await Verify(arg)
            .UseParameters(arg);

    [Theory]
    [InlineData((float) 1.1)]
    public async Task Float(float arg) =>
        await Verify(arg)
            .UseParameters(arg);

    [Theory]
    [InlineData(1.1d)]
    public async Task Double(double arg) =>
        await Verify(arg)
            .UseParameters(arg);

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
    public Task InlineDataUsageFluent(string arg) =>
        Verify(arg)
            .UseParameters(arg);

    #endregion

    #region UseParameters

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

    #region UseParametersSubSet

    [Theory]
    [InlineData("Value1", "Value2", "Value3")]
    public Task UseParametersSubSet(string arg1, string arg2, string arg3)
    {
        var somethingToVerify = $"{arg1} {arg2} {arg3} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg1, arg2);
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
    public Task MemberDataUsageFluent(string arg) =>
        Verify(arg)
            .UseParameters(arg);

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[]
        {
            "Value1"
        };
        yield return new object[]
        {
            "Value2"
        };
    }

    #endregion

    #region xunitAutoFixture

    [Theory]
    [InlineAutoData(42)]
    public Task AutoFixtureUsage(int stable, string random1, string random2)
    {
        var result = MethodBeingTested(stable, random1, random2);
        return Verify(result)
            .UseParameters(stable);
    }

    #endregion

    static int MethodBeingTested(int stable, string random1, string random2) =>
        stable;
}