public class OmitContentFromExceptionTests :
    BaseTest
{
    public OmitContentFromExceptionTests()
    {
        VerifierSettings.OmitContentFromException();
        VerifierSettings.DisableRequireUniquePrefix();
    }
    //TODO: work out the race condition here
    // [Fact]
    // public async Task String()
    // {
    //     try
    //     {
    //         await Verify("Foo")
    //             .DisableDiff();
    //     }
    //     catch (Exception exception)
    //     {
    //         await Verify(exception.Message);
    //     }
    // }
}