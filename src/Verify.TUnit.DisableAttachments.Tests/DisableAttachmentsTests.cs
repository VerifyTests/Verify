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
        var list = GetAttachments();
        await Assert.That(list.Count).IsEqualTo(0);
    }

    static List<Artifact> GetAttachments()
    {
        var context = TestContext.Current!;
        var field = typeof(TestContext)
            .GetField("Artifacts", BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (List<Artifact>)field.GetValue(context)!;
    }
}