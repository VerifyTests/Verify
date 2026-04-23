public class AttachmentTests
{
#if NET11_0
    [Fact]
    public async Task Simple()
    {
        DiffEngine.BuildServerDetector.Detected = true;
        var path = CurrentFile.Relative("AttachmentTests.Simple.verified.txt");
        var fullPath = Path.GetFullPath(path);
        File.Delete(fullPath);
        await Verify("Foo").AutoVerify();
        File.Delete(fullPath);
        var key = Assert.Single(TestContext.Current.Attachments!).Key;
        Assert.StartsWith("Verify.XunitV3.Tests/AttachmentTests.Simple.", key);
        Assert.EndsWith(".received.txt", key);
        Assert.DoesNotContain(':', key);
    }

    [Fact]
    public async Task NestedWithSameName()
    {
        DiffEngine.BuildServerDetector.Detected = true;
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

        var key = Assert.Single(TestContext.Current.Attachments!).Key;
        Assert.StartsWith("Verify.XunitV3.Tests/AttachmentTests/AttachmentTests.Simple.", key);
        Assert.EndsWith(".received.txt", key);
        Assert.DoesNotContain(':', key);
    }
#endif
}