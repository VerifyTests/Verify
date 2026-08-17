public class ThrowIfVerifyHasBeenRunTests
{
    // The message points at the API that must move to a module initializer, so it has to
    // name that API and not the code that called it
    [Fact]
    public void NamesTheApi()
    {
        var original = InnerVerifier.verifyHasBeenRun;
        InnerVerifier.verifyHasBeenRun = true;
        try
        {
            var exception = Assert.Throws<Exception>(
                () => VerifierSettings.IgnoreMembers<string>("TheMember"));

            Assert.Contains("The API 'IgnoreMembers'", exception.Message);
            Assert.DoesNotContain(nameof(NamesTheApi), exception.Message);
        }
        finally
        {
            InnerVerifier.verifyHasBeenRun = original;
        }
    }
}
