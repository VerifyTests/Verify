#if DEBUG
using DiffEngine;

public partial class Tests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task Text(
        bool hasExistingReceived,
        bool autoVerify)
    {
        var concat = ParameterBuilder.Concat(new()
        {
            {"hasExistingReceived", hasExistingReceived},
            {"autoVerify", autoVerify},
        });
        var uniqueTestName = $"Tests.Text_{concat}";
        var settings = new VerifySettings();
        settings.UseParameters(hasExistingReceived, autoVerify);
        await RunTest(
            "txt",
            () => "someText",
            () => "someOtherText",
            hasMatchingDiffTool: true,
            hasExistingReceived,
            autoVerify,
            settings,
            uniqueTestName);
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(false, false, true)]
    [InlineData(true, true, true)]
    public async Task Stream(
        bool hasMatchingDiffTool,
        bool hasExistingReceived,
        bool autoVerify)
    {
        var extension = hasMatchingDiffTool ? "knownBin" : "unknownBin";

        var concat = ParameterBuilder.Concat(new()
        {
            {"hasMatchingDiffTool", hasMatchingDiffTool},
            {"hasExistingReceived", hasExistingReceived},
            {"autoVerify", autoVerify},
        });
        var uniqueTestName = $"Tests.Stream_{concat}";
        var settings = new VerifySettings();
        settings.UseParameters(hasMatchingDiffTool, hasExistingReceived, autoVerify);
        await RunTest(
            extension: extension,
            () => new MemoryStream(new byte[] {1}),
            () => new MemoryStream(new byte[] {2}),
            hasMatchingDiffTool,
            hasExistingReceived,
            autoVerify,
            settings,
            uniqueTestName);
    }

    static async Task RunTest(
        string extension,
        Func<object> initialTarget,
        Func<object> secondTarget,
        bool hasMatchingDiffTool,
        bool hasExistingReceived,
        bool autoVerify,
        VerifySettings settings,
        string uniqueTestName)
    {
        settings.UseExtension(extension);
        if (autoVerify)
        {
            settings.AutoVerify();
        }

        var projectDirectory = AttributeReader.GetProjectDirectory();
        var prefix = Path.Combine(projectDirectory, uniqueTestName);
        var danglingFile = Path.Combine(projectDirectory, $"{prefix}.01.verified.{extension}");
        var file = new FilePair(extension, prefix);

        DeleteAll(danglingFile, file.VerifiedPath, file.ReceivedPath);
        await File.WriteAllTextAsync(danglingFile, "");

        if (hasExistingReceived)
        {
            await File.WriteAllTextAsync(file.ReceivedPath, "");
        }

        PrefixUnique.Clear();
        await InitialVerify(initialTarget, hasMatchingDiffTool, settings, file);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        AssertNotExists(danglingFile);

        PrefixUnique.Clear();
        await ReVerify(initialTarget, settings, file);

        PrefixUnique.Clear();
        await InitialVerify(secondTarget, hasMatchingDiffTool, settings, file);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        PrefixUnique.Clear();
        await ReVerify(secondTarget, settings, file);
    }

    static async Task ReVerify(Func<object> target, VerifySettings settings, FilePair pair)
    {
        var command = BuildCommand(pair);
        ProcessCleanup.Refresh();
        await Verify(target(), settings);
        await Task.Delay(300);
        ProcessCleanup.Refresh();
        AssertProcessNotRunning(command);

        AssertNotExists(pair.ReceivedPath);
        AssertExists(pair.VerifiedPath);

        await EnsureUtf8(pair);
    }

    static byte[] preamble = Encoding.UTF8.GetPreamble();

    static async Task EnsureUtf8(FilePair pair)
    {
        if (pair.Extension != "txt")
        {
            return;
        }

        static async Task Ensure(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var bytes = await File.ReadAllBytesAsync(path);
            if (bytes.Length < preamble.Length ||
                preamble.Where((p, i) => p != bytes[i]).Any())
            {
                throw new ArgumentException("Not utf8-BOM");
            }
        }

        await Ensure(pair.VerifiedPath);
        await Ensure(pair.ReceivedPath);
    }

    static async Task InitialVerify(Func<object> target, bool hasMatchingDiffTool, VerifySettings settings, FilePair pair)
    {
        if (settings.autoVerify)
        {
            await Verify(target(), settings);
            AssertExists(pair.VerifiedPath);
        }
        else
        {
            await Throws(() => Verify(target(), settings));
            ProcessCleanup.Refresh();
            AssertProcess(hasMatchingDiffTool, pair);
            if (hasMatchingDiffTool)
            {
                AssertExists(pair.VerifiedPath);
            }

            AssertExists(pair.ReceivedPath);
        }

        await EnsureUtf8(pair);
    }
}
#endif