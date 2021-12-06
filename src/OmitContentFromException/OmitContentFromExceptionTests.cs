[UsesVerify]
public class OmitContentFromExceptionTests
{
    static OmitContentFromExceptionTests()
    {
        VerifierSettings.OmitContentFromException();
        VerifierSettings.DisableRequireUniquePrefix();
    }

    [Fact]
    public async Task String()
    {
        try
        {
            await Verifier.Verify("Foo").DisableDiff();
        }
        catch (Exception exception)
        {
            await Verifier.Verify(exception.Message).ScrubLinesContaining("DiffEngineTray");
        }
    }
}