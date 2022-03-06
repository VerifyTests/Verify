// ReSharper disable MethodHasAsyncOverload

using DiffEngine;

[UsesVerify]
public class StreamTests
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.RegisterFileConverter("abc", ConvertAbc);
        VerifierSettings.RegisterFileConverter("xyz", ConvertXyz);
        VerifierSettings.RegisterFileConverter(ConvertAbcBinary, (o, extension, _) => IsBinary(o, extension, "abc"));
        VerifierSettings.RegisterFileConverter(ConvertXyzBinary, (o, extension, _) => IsBinary(o, extension, "xyz"));

        DiffTools.AddTool("NullComparer",
            true, false, true, true,
            (tempFile, targetFile) => $"\"{targetFile}\" \"{tempFile}\"",
            (tempFile, targetFile) => $"\"{tempFile}\" \"{targetFile}\"",
            exePath: "NullComparer.exe",
            binaryExtensions: new[] { "abc", "xyz" });

        DiffRunner.MaxInstancesToLaunch(int.MaxValue);
    }

    internal static bool RunningOnBuildServer => BuildServerDetector.Detected;

    static readonly string[] ExpectedAbcFiles =
    {
        "00.received.txt",
        "00.verified.txt",
        "01.received.abc",
        "01.verified.abc"
    };

    static readonly string[] ExpectedXyzFiles = ExpectedAbcFiles.Select(item => item.Replace(".abc", ".xyz")).ToArray();

    const string ExpectedText = "abc";

    static readonly Binary Binary1 = new("abc", "123");
    static readonly Binary Binary2 = new("abc", "1234");

    [Fact]
    public async Task VerifyFailsCorrectly_NoFilesExist()
    {
        var context = CreateContext();

        var filesBeforeTest = new HashSet<string>(context.GetFileKeys());

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(Binary1);
        });

        var expectedFiles = ExpectedAbcFiles;

        Assert.Equal(expectedFiles.InTestContext(filesBeforeTest), context.GetFileKeys());

        await context.AssertDiffToolExecutedAsync(2);
    }

    [Fact]
    public async Task VerifyFailsCorrectly_NoFilesExist_UsingExtension()
    {
        var context = CreateContext();

        var filesBeforeTest = new HashSet<string>(context.GetFileKeys());

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(Binary1).UseExtension("xyz");
        });

        var expectedFiles = ExpectedXyzFiles;

        Assert.Equal(expectedFiles.InTestContext(filesBeforeTest), context.GetFileKeys());

        await context.AssertDiffToolExecutedAsync(2);
    }

    [Fact]
    public async Task VerifyFailsCorrectly_TextFileExistsWithDifferentContent_BinaryIsMissing()
    {
        var context = CreateContext();

        var expectedFiles = ExpectedAbcFiles;

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, "dummy");

        var filesBeforeTest = new HashSet<string>(context.GetFileKeys());

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(Binary1);
        });

        Assert.Equal(expectedFiles.InTestContext(filesBeforeTest), context.GetFileKeys());

        await context.AssertDiffToolExecutedAsync(2);
    }

    [Fact]
    public async Task VerifyFailsCorrectly_TextFileExistsWithDifferentContent_BinaryExistsWithDifferentContent()
    {
        var context = CreateContext();

        var expectedFiles = ExpectedAbcFiles;
        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, "dummy");
        var binaryFile = context.GetFullName(expectedFiles[3]);
        Binary2.SaveAs(binaryFile);

        var filesBeforeTest = new HashSet<string>(context.GetFileKeys());

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(Binary1);
        });

        Assert.Equal(expectedFiles.InTestContext(filesBeforeTest), context.GetFileKeys());

        await context.AssertDiffToolExecutedAsync(2);
    }

    [Fact]
    public async Task VerifyFailsCorrectly_TextExistAndMatching_BinaryIsMissing()
    {
        var context = CreateContext();

        var expectedFiles = ExpectedAbcFiles;

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, ExpectedText);

        var filesBeforeTest = new HashSet<string>(context.GetFileKeys());

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(Binary1);
        });

        Assert.Equal(expectedFiles.Skip(1).InTestContext(filesBeforeTest), context.GetFileKeys());

        await context.AssertDiffToolExecutedAsync(1);
    }


    [Fact]
    public async Task VerifyFailsCorrectly_TextExistAndMatching_BinaryExistsWithDifferentContent()
    {
        var context = CreateContext();

        var expectedFiles = ExpectedAbcFiles;

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, ExpectedText);
        var binaryFile = context.GetFullName(expectedFiles[3]);
        Binary2.SaveAs(binaryFile);

        var filesBeforeTest = new HashSet<string>(context.GetFileKeys());

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(Binary1);
        });

        Assert.Equal(expectedFiles.Skip(1).InTestContext(filesBeforeTest), context.GetFileKeys());

        await context.AssertDiffToolExecutedAsync(1);
    }

    [Fact]
    public async Task VerifyFailsCorrectly_TextExistAndMatching_BinaryExistsWithDifferentContent_UsingExtension()
    {
        var context = CreateContext();

        var expectedFiles = ExpectedXyzFiles;

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, ExpectedText);
        var binaryFile = context.GetFullName(expectedFiles[3]);
        Binary2.SaveAs(binaryFile);

        var filesBeforeTest = new HashSet<string>(context.GetFileKeys());

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(Binary1).UseExtension("xyz");
        });

        Assert.Equal(expectedFiles.Skip(1).InTestContext(filesBeforeTest), context.GetFileKeys());

        await context.AssertDiffToolExecutedAsync(1);
    }

    [Fact]
    public async Task VerifySucceeds_TextAndBinaryMatching()
    {
        var context = CreateContext();

        var expectedFiles = ExpectedAbcFiles;

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, ExpectedText);
        var binaryFile = context.GetFullName(expectedFiles[3]);

        Binary1.SaveAs(binaryFile);

        var filesBeforeTest = new HashSet<string>(context.GetFileKeys());

        await Verify(Binary1);

        Assert.Equal(expectedFiles.Where(file => file.Contains("verified")).InTestContext(filesBeforeTest), context.GetFileKeys());

        await context.AssertDiffToolExecutedAsync(0);
    }

    Context<StreamTests> CreateContext([CallerMemberName] string? testMethodName = null, [CallerFilePath] string? sourceFile = null)
    {
        var context = new Context<StreamTests>(testMethodName!, sourceFile!);

        context.ClearFiles();

        return context;
    }

    class Context<T>
    {
        public Context(string testMethodName, string sourceFile)
        {
            FilePrefix = $"{typeof(T).Name}.{testMethodName}.";
            Folder = Path.GetDirectoryName(sourceFile)!;
        }

        public string FilePrefix { get; }

        public string Folder { get; }

        public string SearchPattern => FilePrefix + "*";

        public IEnumerable<string> GetFileKeys()
        {
            return GetFiles()
                .Select(file => Path.GetFileName(file).Substring(FilePrefix.Length))
                .OrderBy(key => key);
        }

        public void ClearFiles()
        {
            var files = GetFiles();
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        public IEnumerable<string> GetFiles()
        {
            return Directory.EnumerateFiles(Folder, SearchPattern);
        }

        public string GetFullName(string file)
        {
            return Path.Combine(Folder, FilePrefix + file);
        }

        public async Task AssertDiffToolExecutedAsync(int expectedFileCount)
        {
            if (RunningOnBuildServer)
            {
                return;
            }

            while (Process.GetProcessesByName("NullComparer").Any())
            {
                await Task.Delay(200);
            }

            var verifiedFiles = GetFiles().Where(item => item.Contains(".verified."));
            var checkedFiles = 0;

            foreach (var verified in verifiedFiles)
            {
                var received = verified.Replace(".verified.", ".received.");
                if (!File.Exists(received))
                    continue;

                checkedFiles++;
                Assert.Equal(NullComparer.Constants.KeyText, File.ReadAllText(verified));
            }

            Assert.Equal(expectedFileCount, checkedFiles);
        }
    }

    class Binary
    {
        public Binary(string metadata, string content)
        {
            Metadata = metadata;
            Content = content;
        }

        public string Metadata { get; set; }

        public string Content { get; set; }

        public void SaveAs(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this));
        }
    }

    static bool IsBinary(object target, string? extension, string requiredExtension)
    {
        return target is Binary && (extension == null || requiredExtension == extension);
    }

    static ConversionResult ConvertXyzBinary(object binary, IReadOnlyDictionary<string, object> context)
    {
        return Convert((Binary)binary, "xyz");
    }

    static ConversionResult ConvertAbcBinary(object binary, IReadOnlyDictionary<string, object> context)
    {
        return Convert((Binary)binary, "abc");
    }

    static ConversionResult Convert(Binary binary, string extension)
    {
        var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.ASCII, 256, true);
        writer.Write(JsonConvert.SerializeObject(binary));
        var info = binary.Metadata;
        stream.Position = 0;
        return new(info, extension, stream);
    }

    static ConversionResult ConvertXyz(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        return Convert(stream, "xyz");
    }

    static ConversionResult ConvertAbc(Stream stream, IReadOnlyDictionary<string, object> context)
    {
        return Convert(stream, "abc");
    }

    static ConversionResult Convert(Stream stream, string extension)
    {
        using var reader = new StreamReader(stream, Encoding.ASCII, true, -1, true);
        var binary = JsonConvert.DeserializeObject<Binary>(reader.ReadToEnd());
        stream.Position = 0;
        var info = binary?.Metadata;
        return new(info, extension, stream);
    }
}

static class ExtensionMethods
{
    public static IEnumerable<string> InTestContext(this IEnumerable<string> files, ICollection<string> filesBeforeTest)
    {
        return files.Where(key => ExistsInTestContext(key, filesBeforeTest));
    }

    static bool ExistsInTestContext(string fileName, ICollection<string> filesBefore)
    {
        return !StreamTests.RunningOnBuildServer || filesBefore.Contains(fileName) || !fileName.Contains(".verified.");
    }

}