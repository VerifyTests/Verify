public class VerifyFilesTests
{
    #region VerifyFiles

    [Fact]
    public Task Run() =>
        VerifyFiles(["File1.txt", "File2.txt"]);

    #endregion

    #region VerifyFilesWithInfo

    [Fact]
    public Task WithInfo() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            info: new
            {
                Key = "Value"
            });

    #endregion

    #region VerifyFilesWithFileScrubber

    [Fact]
    public Task WithFileScrubber() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            fileScrubber: (_, builder) =>
            {
                builder.Clear();
                builder.Append("New");
            });

    #endregion
}