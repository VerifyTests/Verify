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

    // Plugins compiled against Verify 32.0.0-beta.8 and earlier emit a call to the
    // parameterless signature, so dropping it fails them with a MissingMethodException
    [Fact]
    public void ParameterlessOverloadRetainedForCompiledPlugins()
    {
        var method = typeof(InnerVerifier).GetMethod(
            nameof(InnerVerifier.ThrowIfVerifyHasBeenRun),
            BindingFlags.Public | BindingFlags.Static,
            null,
            [],
            null);

        Assert.NotNull(method);
    }
}
