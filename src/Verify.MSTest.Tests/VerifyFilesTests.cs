[TestClass]
public partial class VerifyFilesTests
{
    [TestMethod]
    public Task Run() =>
        VerifyFiles(["File1.txt", "File2.txt"]);

    [TestMethod]
    public Task WithInfo() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            info: new
            {
                Key = "Value"
            });

    [TestMethod]
    public Task WithFileScrubber() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            fileScrubber: (_, builder) =>
            {
                builder.Clear();
                builder.Append("New");
            });
}