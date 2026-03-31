public class IgnoreClassArgumentsTests :
    BaseTest
{
    public IgnoreClassArgumentsTests() =>
        VerifierSettings.IgnoreClassArguments();

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreClassArguments(string classArg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(classArg);
        settings.SetClassArgumentCount(1);
        return Verify("value", settings);
    }
}
