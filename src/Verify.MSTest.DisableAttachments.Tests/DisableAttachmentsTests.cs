[TestClass]
public class DisableAttachmentsTests : VerifyBase
{
    [ResultFilesCallback]
    [TestMethod]
    public Task HasNoAttachments()
    {
        ResultFilesCallback.Callback = list => Assert.IsEmpty(list);
        VerifierSettings.DisableAttachments();
        var settings = new VerifySettings();
        settings.DisableDiff();
        return Assert.ThrowsExactlyAsync<VerifyException>(
            () => Verify("Bar", settings));
    }
}