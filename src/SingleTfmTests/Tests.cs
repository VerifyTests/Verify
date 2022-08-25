[UsesVerify]
public class Tests
{
    [Fact]
    public async Task Simple()
    {
        var verified = CurrentFile.Relative("Tests.Simple.verified.txt");
        var received = CurrentFile.Relative("Tests.Simple.received.txt");
        File.Delete(verified);
        File.Delete(received);
        var settings = new VerifySettings();
        settings.DisableDiff();
        try
        {
            await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));
            Assert.True(File.Exists(received));
        }
        finally
        {
            File.Delete(verified);
            File.Delete(received);
        }
    }
}