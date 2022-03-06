// ReSharper disable MethodHasAsyncOverload

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
    }

    static readonly string[] expectedFiles =
    {
        "00.received.txt",
        "00.verified.txt",
        "01.received.abc",
        "01.verified.abc"
    };

    const string expectedText = "abc";

    static readonly Binary binary1 = new("abc", "123");
    static readonly Binary binary2 = new("abc", "1234");

    [Fact]
    public async Task VerifyFailsCorrectly_NoFilesExist()
    {
        var context = new Context();

        context.ClearFiles();

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(binary1);
        });

        Assert.Equal(expectedFiles, context.GetFileKeys());
    }

    [Fact]
    public async Task VerifyFailsCorrectly_NoFilesExist_UsingExtension()
    {
        var context = new Context();

        context.ClearFiles();

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(binary1).UseExtension("xyz");
        });

        Assert.Equal(expectedFiles, context.GetFileKeys());
    }

    [Fact]
    public async Task VerifyFailsCorrectly_TextFileExistsWithDifferentContent_BinaryIsMissing()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, "dummy");

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(binary1);
        });

        Assert.Equal(expectedFiles.Take(3), context.GetFileKeys());
    }

    [Fact]
    public async Task VerifyFailsCorrectly_TextFileExistsWithDifferentContent_BinaryExistsWithDifferentContent()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, "dummy");
        var binaryFile = context.GetFullName(expectedFiles[3]);
        binary2.SaveAs(binaryFile);

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(binary1);
        });

        Assert.Equal(expectedFiles, context.GetFileKeys());
    }

    [Fact]
    public async Task VerifyFailsCorrectly_TextExistAndMatching_BinaryIsMissing()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, expectedText);

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(binary1);
        });

        Assert.Equal(expectedFiles.Skip(1), context.GetFileKeys());
    }


    [Fact]
    public async Task VerifyFailsCorrectly_TextExistAndMatching_BinaryExistsWithDifferentContent()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, expectedText);
        var binaryFile = context.GetFullName(expectedFiles[3]);
        binary2.SaveAs(binaryFile);

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(binary1);
        });

        Assert.Equal(expectedFiles.Skip(1), context.GetFileKeys());
    }

    [Fact]
    public async Task VerifyFailsCorrectly_TextExistAndMatching_BinaryExistsWithDifferentContent_UsingExtension()
    {
        var context = new Context();

        context.ClearFiles();

        var xyzFiles = expectedFiles.Select(item => item.Replace(".abc", ".xyz")).ToArray();

        var textFile = context.GetFullName(xyzFiles[1]);
        File.WriteAllText(textFile, expectedText);
        var binaryFile = context.GetFullName(xyzFiles[3]);
        binary2.SaveAs(binaryFile);

        var ex = await Assert.ThrowsAsync<VerifyException>(async () =>
        {
            await Verify(binary1).UseExtension("xyz");
        });

        Assert.Equal(xyzFiles, context.GetFileKeys());
    }

    [Fact]
    public async Task VerifySucceeds_TextAndBinaryMatching()
    {
        var context = new Context();

        context.ClearFiles();

        var textFile = context.GetFullName(expectedFiles[1]);
        File.WriteAllText(textFile, expectedText);
        var binaryFile = context.GetFullName(expectedFiles[3]);

        binary1.SaveAs(binaryFile);

        await Verify(binary1);

        Assert.Equal(expectedFiles.Where(file => file.Contains("verified")), context.GetFileKeys());
    }

    class Context
    {
        public Context([CallerMemberName] string? testMethodName = null, [CallerFilePath] string sourceFile = "")
        {
            FilePrefix = $"{nameof(StreamTests)}.{testMethodName}.";
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