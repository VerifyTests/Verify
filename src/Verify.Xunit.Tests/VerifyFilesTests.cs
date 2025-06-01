public class VerifyFilesTests
{
    #region VerifyFilesXunit

    [Fact]
    public Task Run() =>
        VerifyFiles(["File1.txt", "File2.txt"]);

    #endregion
}