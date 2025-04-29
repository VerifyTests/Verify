public class InnerVerifyTests
{
    static string splitFilePath;
    static string filePath;
    static string targetDirectory;

    static InnerVerifyTests()
    {
        var solutionDirectory = AttributeReader.GetSolutionDirectory();
        targetDirectory = Path.Combine(solutionDirectory, "Verify.Tests", "InnerVerifyTests");
        filePath = Path.Combine(targetDirectory, "sample.txt");
        splitFilePath = Path.Combine(targetDirectory, "sample.innersplit");
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

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.RegisterStreamConverter(
            "innersplit",
            (_, stream, _) =>
            {
                var reader = new StreamReader(stream);
                return new(
                    "the info",
                    [
                        new("txt", reader.ReadToEnd()),
                        new("txt", "text1"),
                        new("txt", "text2")
                    ]);
            });

    [Fact]
    public async Task Split()
    {
        using var verifier = new InnerVerifier(targetDirectory, "split");
        await verifier.VerifyFile(splitFilePath);
    }
}