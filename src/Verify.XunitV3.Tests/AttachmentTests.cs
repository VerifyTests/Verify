public class AttachmentTests
{
    [Fact]
    public async Task Simple()
    {
        var path = CurrentFile.Relative("AttachmentTests.Simple.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo").AutoVerify();
        File.Delete(fullPath);
        var attachments = TestContext.Current.Attachments!;
        Assert.Contains(attachments.Keys,
            _ => _.Contains("Verify snapshot mismatch") &&
                 _.Contains("AttachmentTests.Simple."));
    }

    [Fact]
    public async Task NestedWithSameName()
    {
        var path = CurrentFile.Relative("AttachmentTests");
        var fullPath = Path.GetFullPath(path);
        Delete();
        await Verify("Foo")
            .UseDirectory("AttachmentTests")
            .UseMethodName("Simple")
            .AutoVerify();
        Delete();

        void Delete()
        {
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }
        }

        var attachments = TestContext.Current.Attachments!;
        Assert.Contains(attachments.Keys,
            _ => _.Contains("Verify snapshot mismatch") &&
                 _.Contains("AttachmentTests.Simple."));
    }
}