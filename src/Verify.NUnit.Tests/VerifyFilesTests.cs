[TestFixture]
public class VerifyFilesTests
{
    [Test]
    public Task Run() =>
        VerifyFiles(["File1.txt", "File2.txt"]);

    [Test]
    public Task WithInfo() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            info: new
            {
                Key = "Value"
            });

    [Test]
    public Task WithFileScrubber() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            fileScrubber: (_, builder) =>
            {
                builder.Clear();
                builder.Append("New");
            });
}