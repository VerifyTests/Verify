public class IgnoreClassArgumentsTests :
    BaseTest
{
    public IgnoreClassArgumentsTests() =>
        VerifierSettings.IgnoreConstructorParameters();

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreConstructorParameters(string classArg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(classArg);
        settings.SetClassArgumentCount(1);
        return Verify("value", settings);
    }
}
