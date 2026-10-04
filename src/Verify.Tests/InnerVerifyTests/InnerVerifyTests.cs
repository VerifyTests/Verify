public class InnerVerifyTests
{
    static string splitFilePath;
    static string filePath;
    static string targetDirectory;

    static InnerVerifyTests()
    {
        targetDirectory = Path.Combine(ProjectFiles.ProjectDirectory, "InnerVerifyTests");
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

    // Dispose runs the after callbacks, so this constructor has to run the before half
    // too. An unbalanced pair leaves whatever it pushes, sets or counts in the wrong state.
    [Fact]
    public void RunsBeforeAndAfterCallbacks()
    {
        var calls = new List<string>();
        var settings = new VerifySettings();
        settings.OnVerify(
            before: () => calls.Add("before"),
            after: () => calls.Add("after"));

        using (new InnerVerifier(targetDirectory, "callbackBalance", settings))
        {
            Assert.Equal(["before"], calls);
        }

        Assert.Equal(["before", "after"], calls);
    }

    [Fact]
    public async Task VerifyExternalFileLocked()
    {
        // ReSharper disable once UseAwaitUsing
        using var locker = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var verifier = new InnerVerifier(targetDirectory, "sample2");
        await verifier.VerifyFile(filePath);
    }

    [ModuleInitializer]
    public static void Init()
    {
        // Force VerifyXunitV3 to module init to avoid
        // "must be called prior to any Verify has run" race condition
        // since otherwise VerifyExternalFileLocked sets verifyHasBeenRun
        // before the VerifyXunitV3 init
        var assembly = typeof(Verifier).Assembly;
        Trace.WriteLine(assembly);

        VerifierSettings.RegisterStreamConverter(
            "innersplit", (_, stream, _) => BuildConversionResult(stream));
    }

    static ConversionResult BuildConversionResult(Stream stream)
    {
        var reader = new StreamReader(stream);
        return new(
            "the info",
            [
                new("txt", reader.ReadToEnd()),
                new("txt", "text1"),
                new("txt", "text2")
            ]);
    }

    [Fact]
    public async Task Split()
    {
        using var verifier = new InnerVerifier(targetDirectory, "split");
        await verifier.VerifyFile(splitFilePath);
    }

    /// <summary>
    /// A named target that is the only one of its name was written to the file an unnamed one
    /// has, so two of them, each page of a document, went to the same file.
    /// </summary>
    [Fact]
    public async Task NamedTargetsAreNamed()
    {
        using var temp = new TempDirectory();
        var settings = new VerifySettings();
        settings.AutoVerify();
        settings.DisableDiff();
        using var verifier = new InnerVerifier(temp, "named", settings);
        List<Target> targets =
        [
            new("txt", "the first page", "page_0001"),
            new("txt", "the second page", "page_0002")
        ];

        var result = await verifier.Verify(targets);

        Assert.Equal(
            [
                "named#page_0001.verified.txt",
                "named#page_0002.verified.txt"
            ],
            result.Files.Select(Path.GetFileName));
        Assert.Equal("the second page", File.ReadAllText(Path.Combine(temp.Path, "named#page_0002.verified.txt"), Encoding.UTF8));
    }
}