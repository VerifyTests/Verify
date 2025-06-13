[TestFixture]
public class DisableAttachmentsTests
{
    [Test]
    public void HasNoAttachments()
    {
        VerifierSettings.DisableAttachments();
        var settings = new VerifySettings();
        settings.DisableDiff();
        ThrowsAsync<VerifyException>(
            () => Verify("Bar", settings));
        var list = GetAttachments();
        AreEqual(0, list.Count);
    }

    static List<TestAttachment> GetAttachments() =>
        TestExecutionContext.CurrentContext.CurrentResult.TestAttachments.ToList();
}