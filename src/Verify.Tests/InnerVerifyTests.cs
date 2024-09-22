public class InnerVerifyTests
{
    [Fact]
    public void InnerVerifier_FileConstructor()
    {
        const string sourceFile = "path/to/source/file.cs";
        var settings = new VerifySettings();

        var verifier = new InnerVerifier(sourceFile, settings);

        Assert.NotNull(verifier);
    }

    [Fact]

    #region VerifyFileWithoutUnitTest

    public async Task VerifyExternalFile()
    {
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        var settings = new VerifySettings();
        settings.DisableRequireUniquePrefix();

        var sourceFile = Path.Combine(solutionDirectory, "Verify.Tests", "sample.txt");

        using var verifier = new InnerVerifier(sourceFile, settings);
        await verifier.VerifyFile(sourceFile, null, null);
    }

    #endregion
}