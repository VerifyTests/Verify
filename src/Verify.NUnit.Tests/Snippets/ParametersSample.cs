// ReSharper disable once RedundantUsingDirective
using Polyfills;
// ReSharper disable UnusedParameter.Local
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract

#pragma warning disable UseParametersAppender
[TestFixture]
public class ParametersSample
{
    [TestCase("1.1")]
    public Task Decimal(decimal arg) =>
        Verify(arg);

    [TestCase((float) 1.1)]
    public Task Float(float arg) =>
        Verify(arg);

    [TestCase(1.1d)]
    public Task Double(double arg) =>
        Verify(arg);

    #region UseTextForParametersInstanceNunit

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task UseTextForParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.UseTextForParameters(arg);
        return Verify(arg + "UseTextForParameters", settings);
    }

    #endregion

    #region UseTextForParametersFluentNunit

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task UseTextForParametersFluent(string arg) =>
        Verify(arg + "UseTextForParametersFluent")
            .UseTextForParameters(arg);

    #endregion

    #region UseParametersSubSetNunit

    [TestCase("Value1", "Value2", "Value3")]
    public Task UseParametersSubSet(string arg1, string arg2, string arg3)
    {
        var somethingToVerify = $"{arg1} {arg2} {arg3} some text";
        return Verify(somethingToVerify)
            .UseParameters(arg1, arg2);
    }

    #endregion

    #region IgnoreParametersForVerifiedNunit

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerified(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified();
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedFluentNunit

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerifiedFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified();

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsNunit

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerifiedCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParametersForVerified($"Number{arg}");
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersForVerifiedCustomParamsFluentNunit

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersForVerifiedCustomParamsFluent(string arg) =>
        Verify("value")
            .IgnoreParametersForVerified($"Number{arg}");

    #endregion

    #region IgnoreParametersNunit

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParameters(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters();
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersFluentNunit

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersFluent(string arg) =>
        Verify("value")
            .IgnoreParameters();

    #endregion

    #region IgnoreParametersCustomParamsNunit

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersCustomParams(string arg)
    {
        var settings = new VerifySettings();
        settings.IgnoreParameters(nameof(arg));
        return Verify("value", settings);
    }

    #endregion

    #region IgnoreParametersCustomParamsFluentNunit

    [TestCase("One")]
    [TestCase("Two")]
    public Task IgnoreParametersCustomParamsFluent(string arg) =>
        Verify("value")
            .IgnoreParameters(nameof(arg));

    #endregion

    #region NUnitTestCase

    [TestCase("Value1")]
    [TestCase("Value2")]
    public Task TestCaseUsage(string arg) =>
        Verify(arg);

    #endregion

    [TestCase("Value2")]
    public Task SuppliedDoesNotMatchArg(string arg) =>
        Verify("Foo")
            .UseParameters("notTheArg");

    #region UseParametersAppender

    [TestCase("One", "Two")]
    [TestCase("Three", "Four")]
    public Task UseParametersAppender(string arg1, string arg2)
    {
        var settings = new VerifySettings();
        settings.UseParametersAppender((values, counter) =>
            stringBuilder =>
            {
                foreach (var (key, value) in values)
                {
                    stringBuilder.Append($"{key.ToUpper()}={value?.ToString()?.ToLower()}_");
                }
            });
        return Verify("value", settings);
    }

    #endregion

    #region UseParametersAppenderFluent

    [TestCase("One", "Two")]
    [TestCase("Three", "Four")]
    public Task UseParametersAppenderFluent(string arg1, string arg2) =>
        Verify("value")
            .UseParametersAppender((values, counter) =>
                stringBuilder =>
                {
                    foreach (var (key, value) in values)
                    {
                        stringBuilder.Append($"{key.ToUpper()}={value?.ToString()?.ToLower()}_");
                    }
                });

    #endregion
}