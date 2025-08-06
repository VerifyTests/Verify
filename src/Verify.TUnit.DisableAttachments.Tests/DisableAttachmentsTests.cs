public class DisableAttachmentsTests
{
    [Test]
    public async Task HasNoAttachments()
    {
        VerifierSettings.DisableAttachments();
        var settings = new VerifySettings();
        settings.DisableDiff();
        await Assert.ThrowsAsync(
            () => Verify("Bar", settings));
        var list = TestContext.Current!.Artifacts;
        await Assert.That(list.Count).IsEqualTo(0);
    }
}