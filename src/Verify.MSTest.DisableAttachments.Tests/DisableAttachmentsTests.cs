[TestClass]
public class DisableAttachmentsTests : VerifyBase
{
    [ResultFilesCallback]
    [TestMethod]
    public Task HasNoAttachments()
    {
        ResultFilesCallback.Callback = list => Assert.AreEqual(0, list.Count);
        VerifierSettings.DisableAttachments();
        var settings = new VerifySettings();
        settings.DisableDiff();
        return Assert.ThrowsExactlyAsync<VerifyException>(
            () => Verify("Bar", settings));
    }
}