
[TestClass]
public partial class DisableAttachmentsTests
{
    [ResultFilesCallback]
    [TestMethod]
    public async Task HasNoAttachments()
    {
        ResultFilesCallback.Callback = list => Assert.AreEqual(0, list.Count);
        VerifierSettings.DisableAttachments();
        var settings = new VerifySettings();
        settings.DisableDiff();
        await Assert.ThrowsExceptionAsync<VerifyException>(
            () => Verify("Bar", settings));
    }

    [TestCleanup]
    public void ResetStaticSettings()
    {
        VerifierSettings.Reset();
        CombinationSettings.Reset();
    }
}