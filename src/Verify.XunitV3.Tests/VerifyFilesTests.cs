public class VerifyFilesTests
{
    [Fact]
    public Task Run() =>
        VerifyFiles(["File1.txt", "File2.txt"]);

    [Fact]
    public Task WithInfo() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            info: new
            {
                Key = "Value"
            });

    [Fact]
    public Task WithFileScrubber() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            fileScrubber: (_, builder) =>
            {
                builder.Clear();
                builder.Append("New");
            });
}