// disable all test parallelism to avoid test interaction

[assembly: Parallelizable(ParallelScope.None)]
[assembly: LevelOfParallelism(1)]

[TestFixture]
public partial class DisableAttachmentsTests
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

    [TearDown]
    public void ResetStaticSettings()
    {
        VerifierSettings.Reset();
        CombinationSettings.Reset();
    }

    private static List<TestAttachment> GetAttachments() =>
        TestExecutionContext.CurrentContext.CurrentResult.TestAttachments.ToList();
}