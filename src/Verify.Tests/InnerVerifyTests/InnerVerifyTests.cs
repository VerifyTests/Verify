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
        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();
        using var verifier = new InnerVerifier(targetDirectory, "sample", settings);
        await verifier.VerifyFile(filePath, null, null);
    }

    #endregion

    [Fact]
    public async Task VerifyExternalFileLocked()
    {
        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();
        using var locker = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var verifier = new InnerVerifier(targetDirectory, "sample", settings);
        await verifier.VerifyFile(filePath, null, null);
    }
}