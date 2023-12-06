﻿[UsesVerify]
public class OmitContentFromExceptionTests :
    BaseTest
{
    public OmitContentFromExceptionTests()
    {
        VerifierSettings.OmitContentFromException();
        VerifierSettings.DisableRequireUniquePrefix();
    }

    [Fact]
    public async Task String()
    {
        try
        {
            await Verify("Foo")
                .DisableDiff();
        }
        catch (Exception exception)
        {
            await Verify(exception.Message);
        }
    }
}