public class IgnoreParametersTests :
    BaseTest
{
    public IgnoreParametersTests() =>
        VerifierSettings.IgnoreParameters("arg");

    [Theory]
    [InlineData("One")]
    [InlineData("Two")]
    public Task IgnoreParameters(string arg) =>
        Verify("value")
            .UseParameters(arg);
}
