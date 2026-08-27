public class IgnoreAllParametersTests :
    BaseTest
{
    public IgnoreAllParametersTests() =>
        VerifierSettings.IgnoreParameters("first");

    // The empty list is the documented "ignore all parameters" sentinel, so it must win
    // over the global ignore list rather than be merged with it
    [Theory]
    [InlineData("One", "A")]
    [InlineData("Two", "B")]
    public Task EmptyListWinsOverGlobalIgnore(string first, string second)
    {
        var settings = new VerifySettings();
        settings.UseParameters(first, second);
        settings.IgnoreParameters();
        return Verify("value", settings);
    }

    // And over the constructor-parameter names
    [Theory]
    [InlineData("One", "A")]
    [InlineData("Two", "B")]
    public Task EmptyListWinsOverConstructorIgnore(string classArg, string methodArg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(classArg, methodArg);
        settings.SetClassArgumentCount(1);
        settings.IgnoreConstructorParameters();
        settings.IgnoreParameters();
        return Verify("value", settings);
    }
}
