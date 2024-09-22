public class InnerVerifyTests
{
    static string filePath;
    static string targetDirectory;

    static InnerVerifyTests()
    {
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        targetDirectory = Path.Combine(solutionDirectory, "Verify.Tests", "InnerVerifyTests");
        filePath = Path.Combine(targetDirectory, "sample.txt");
    }

    [Fact]

    #region VerifyFileWithoutUnitTest

    public async Task VerifyExternalFile()
    {
        using var verifier = new InnerVerifier(targetDirectory, name: "sample");
        await verifier.VerifyFile(filePath);
    }

    #endregion

    [Fact]
    public async Task VerifyExternalFileLocked()
    {
        using var locker = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var verifier = new InnerVerifier(targetDirectory, "sample2");
        await verifier.VerifyFile(filePath);
    }
}