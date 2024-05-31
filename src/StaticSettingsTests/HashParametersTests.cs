public class HashParametersTests :
    BaseTest
{
    public HashParametersTests() =>
        VerifierSettings.HashParameters();

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public Task HashParametersUsage(bool arg) =>
        Verify(arg)
            .UseParameters(arg);
}