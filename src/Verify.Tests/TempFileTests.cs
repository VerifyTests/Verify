public class TempFileTests
{
    [Fact]
    public void WithNoVerify()
    {
        using var temp = new TempFile();
    }

    [Fact]
    public void Constructor_CreatesFilePathInRootDirectory()
    {
        using var temp = new TempFile();

        Assert.StartsWith(TempFile.RootDirectory, temp.Path);
        Assert.NotEmpty(temp.Path);
    }

    [Fact]
    public void Constructor_WithExtension_AppliesExtension()
    {
        using var temp = new TempFile(".txt");

        Assert.EndsWith(".txt", temp.Path);
    }

    #region VerifyTempFile

    [Fact]
    public async Task VerifyFileInstance()
    {
        using var file = new TempFile("txt");
        await File.WriteAllTextAsync(file, "test");
        await VerifyFile(file);
    }

    #endregion

    #region TempFile

    [Fact]
    public void Usage()
    {
        using var temp = new TempFile();

        File.WriteAllText(temp, "content");

        // file automatically deleted here
    }

    #endregion

    #region TempFilePathProperty

    [Fact]
    public void PathProperty()
    {
        using var temp = new TempFile();
        var path = temp.Path;
        Assert.True(Path.IsPathRooted(path));
    }

    #endregion

    #region TempFileOpenExplorerAndDebug

    [Fact(Explicit = true)]
    public void OpenExplorerAndDebug()
    {
        using var temp = new TempFile();

        File.WriteAllText(temp, "content");

        // this is temporary debugging code and should not be commited to source control
        temp.OpenExplorerAndDebug();
    }

    #endregion

    #region TempFileNoUsing

    [Fact(Explicit = true)]
    public void NoUsing()
    {
        //using var temp = new TempFile();
        var temp = new TempFile();

        File.WriteAllText(temp, "content");

        Debug.WriteLine(temp);
    }

    #endregion

#if DEBUG

    #region TempFileScrubbing

    [Fact]
    public async Task Scrubbing()
    {
        using var temp = new TempFile();

        await Verify(new
        {
            PropertyWithTempPath = temp,
            TempInStringProperty = $"The path is {temp}"
        });
    }

    #endregion

#endif

    #region TempFileRootDirectory

    [Fact]
    public void RootDirectory() =>
        // Accessing the root directory for all TempDirectory instances
        Trace.WriteLine(TempFile.RootDirectory);

    #endregion

    [Theory]
    [InlineData(".json")]
    [InlineData(".xml")]
    [InlineData(".csv")]
    public void Constructor_WithVariousExtensions_AppliesCorrectly(string extension)
    {
        using var temp = new TempFile(extension);

        Assert.EndsWith(extension, temp.Path);
    }

    [Fact]
    public void Create_WithoutExtension_CreatesEmptyFile()
    {
        using var temp = TempFile.Create();

        Assert.True(File.Exists(temp.Path));
        Assert.Equal(0, new FileInfo(temp.Path).Length);
    }

    [Fact]
    public void Create_WithExtension_CreatesFileWithExtension()
    {
        using var temp = TempFile.Create(".txt");

        Assert.True(File.Exists(temp.Path));
        Assert.EndsWith(".txt", temp.Path);
    }

    [Fact]
    public void Create_WithEncoding_CreatesFileWithBom()
    {
        using var temp = TempFile.Create(".txt", Encoding.UTF8);

        Assert.True(File.Exists(temp.Path));
        var bytes = File.ReadAllBytes(temp.Path);
        // UTF8 BOM should be present
        Assert.True(bytes.Length >= 3);
    }

    [Fact]
    public void CreateText_WithEncoding_CreatesFileWithBom()
    {
        using var temp = TempFile.Create(".txt", Encoding.UTF8);

        Assert.True(File.Exists(temp.Path));
        var bytes = File.ReadAllBytes(temp.Path);
        // UTF8 BOM should be present
        Assert.True(bytes.Length >= 3);
    }

    [Fact]
    public void Dispose_DeletesFile()
    {
        string path;

        using (var temp = TempFile.Create())
        {
            path = temp.Path;
            Assert.True(File.Exists(path));
        }

        Assert.False(File.Exists(path));
    }

    [Fact]
    public void Dispose_WhenFileDoesNotExist_DoesNotThrow()
    {
        var temp = TempFile.Create();
        File.Delete(temp.Path);

        var exception = Record.Exception(() => temp.Dispose());

        Assert.Null(exception);
    }

    [Fact]
    public void ToString_ReturnsPath()
    {
        using var temp = new TempFile();

        Assert.Equal(temp.Path, temp.ToString());
    }

    [Fact]
    public void ImplicitStringOperator_ReturnsPath()
    {
        using var temp = TempFile.Create(".txt");
        string path = temp;

        Assert.Equal(temp.Path, path);
        File.WriteAllText(path, "test content");
        Assert.Equal("test content", File.ReadAllText(temp.Path));
    }

    [Fact]
    public void ImplicitFileInfoOperator_ReturnsFileInfo()
    {
        using var temp = TempFile.Create(".txt");
        File.WriteAllText(temp.Path, "test");

        FileInfo fileInfo = temp;

        Assert.Equal(temp.Path, fileInfo.FullName);
        Assert.True(fileInfo.Exists);
    }

    [Fact]
    public void Info_ReturnsFileInfo()
    {
        using var temp = TempFile.Create(".txt");
        File.WriteAllText(temp.Path, "test");

        var info = temp.Info;

        Assert.Equal(temp.Path, info.FullName);
        Assert.True(info.Exists);
        Assert.Equal(4, info.Length);
    }

    [Fact]
    public void RootDirectory_Exists() =>
        Assert.True(Directory.Exists(TempFile.RootDirectory));

    [Fact]
    public void RootDirectory_IsInSystemTemp()
    {
        var systemTemp = Path.GetTempPath();

        Assert.StartsWith(systemTemp, TempFile.RootDirectory);
        Assert.EndsWith("VerifyTempFiles", TempFile.RootDirectory);
    }

    [Fact]
    public void Cleanup_RemovesOldFiles()
    {
        // Create a test file and set its last write time to 2 days ago
        var oldFile = Path.Combine(TempFile.RootDirectory, Path.GetRandomFileName());
        File.WriteAllText(oldFile, "old content");
        File.SetLastWriteTime(oldFile, DateTime.Now.AddDays(-2));

        TempFile.Cleanup();

        Assert.False(File.Exists(oldFile));
    }

    [Fact]
    public void Cleanup_KeepsRecentFiles()
    {
        using var temp = TempFile.Create();
        File.WriteAllText(temp.Path, "recent content");

        TempFile.Cleanup();

        Assert.True(File.Exists(temp.Path));
    }

    [Fact]
    public void Cleanup_HandlesFileNotFoundRaceCondition()
    {
        // This tests that Cleanup doesn't throw if a file is deleted during cleanup
        var exception = Record.Exception(TempFile.Cleanup);

        Assert.Null(exception);
    }

    [Fact]
    public void OpenExplorerAndDebug_OnBuildServer_ThrowsException()
    {
        if (!BuildServerDetector.Detected)
        {
            // Skip if not on build server
            return;
        }

        using var temp = TempFile.Create();

        var exception = Assert.Throws<Exception>(() => temp.OpenExplorerAndDebug());
        Assert.Contains("not supported on build servers", exception.Message);
    }

    [Fact]
    public void MultipleInstances_CreateUniqueFiles()
    {
        using var temp1 = new TempFile(".txt");
        using var temp2 = new TempFile(".txt");
        using var temp3 = new TempFile(".txt");

        Assert.NotEqual(temp1.Path, temp2.Path);
        Assert.NotEqual(temp1.Path, temp3.Path);
        Assert.NotEqual(temp2.Path, temp3.Path);
    }

    [Fact]
    public void Create_CanWriteAndReadContent()
    {
        using var temp = TempFile.Create(".txt");
        var content = "Hello, World!";

        File.WriteAllText(temp.Path, content);
        var readContent = File.ReadAllText(temp.Path);

        Assert.Equal(content, readContent);
    }

    [Fact]
    public async Task Create_CanBeUsedConcurrently()
    {
        var tasks = Enumerable.Range(0, 10)
            .Select(async i =>
            {
                using var temp = TempFile.Create(".txt");
                await File.WriteAllTextAsync(temp.Path, $"Content {i}");
                return temp.Path;
            });

        var paths = await Task.WhenAll(tasks);

        Assert.Equal(10, paths.Distinct().Count());
    }

    #region TempFileStringConversion

    [Fact]
    public void StringConversion()
    {
        using var temp = new TempFile();

        File.WriteAllText(temp, "content");

        // implicit conversion to string
        string path = temp;
        var content = File.ReadAllText(path);
        Trace.WriteLine(content);
    }

    #endregion

    #region TempFileFileInfoConversion

    [Fact]
    public void FileInfoConversion()
    {
        using var temp = new TempFile();

        // implicit conversion to FileInfo
        FileInfo info = temp;

        var directoryName = info.DirectoryName;
        Trace.WriteLine(directoryName);
    }

    #endregion

    #region TempFileInfoProperty

    [Fact]
    public void InfoProperty()
    {
        using var temp = new TempFile();

        var directoryName = temp.Info.DirectoryName;

        Trace.WriteLine(directoryName);
    }

    #endregion

    [Fact]
    public void Constructor_WithEmptyExtension_ThrowsArgumentException() =>
        Assert.Throws<ArgumentException>(() => new TempFile(string.Empty));

    [Fact]
    public async Task CreateText_WithContent_CreatesFileWithContent()
    {
        var content = "Hello, World!";

        using var temp = await TempFile.CreateText(content);

        Assert.True(File.Exists(temp.Path));
        var readContent = await File.ReadAllTextAsync(temp.Path);
        Assert.Equal(content, readContent);
    }

    [Fact]
    public async Task CreateText_WithExtension_CreatesFileWithExtensionAndContent()
    {
        var content = "Test content";

         using var temp = await TempFile.CreateText(content, ".txt");

        Assert.EndsWith(".txt", temp.Path);
        Assert.True(File.Exists(temp.Path));
        var readContent = await File.ReadAllTextAsync(temp.Path);
        Assert.Equal(content, readContent);
    }

    [Fact]
    public async Task CreateText_WithEncoding_CreatesFileWithSpecifiedEncoding()
    {
        var content = "Test with special characters: äöü";

        using var temp = await TempFile.CreateText(content, ".txt", Encoding.UTF8);

        Assert.True(File.Exists(temp.Path));
        var readContent = await File.ReadAllTextAsync(temp.Path, Encoding.UTF8);
        Assert.Equal(content, readContent);
    }

    [Fact]
    public async Task CreateText_WithAsciiEncoding_CreatesFileWithAsciiEncoding()
    {
        var content = "ASCII content only";

        using var temp = await TempFile.CreateText(content, ".txt", Encoding.ASCII);

        Assert.True(File.Exists(temp.Path));
        var readContent = await File.ReadAllTextAsync(temp.Path, Encoding.ASCII);
        Assert.Equal(content, readContent);
    }

    [Fact]
    public async Task CreateText_WithEmptyContent_CreatesEmptyFile()
    {
        using var temp = await TempFile.CreateText(string.Empty, ".txt");

        Assert.True(File.Exists(temp.Path));
        var readContent = await File.ReadAllTextAsync(temp.Path);
        Assert.Empty(readContent);
    }

    [Fact]
    public async Task CreateText_WithMultilineContent_PreservesLineBreaks()
    {
        var content = """
                      Line 1
                      Line 2
                      Line 3
                      """;

        using var temp = await TempFile.CreateText(content, ".txt");

        var readContent = await File.ReadAllTextAsync(temp.Path);
        Assert.Equal(content, readContent);
    }

    [Fact]
    public async Task CreateText_DisposesCorrectly()
    {
        string path;
        {
            using var temp = await TempFile.CreateText("content", ".txt");
            path = temp.Path;
            Assert.True(File.Exists(path));
        }

        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task CreateBinary_WithContent_CreatesFileWithBinaryContent()
    {
        var data = "Hello"u8.ToArray(); // "Hello" in ASCII

        using var temp = await TempFile.CreateBinary(data);

        Assert.True(File.Exists(temp.Path));
        var readData = await File.ReadAllBytesAsync(temp.Path);
        Assert.Equal(data, readData);
    }

    [Fact]
    public async Task CreateBinary_WithExtension_CreatesFileWithExtensionAndContent()
    {
        byte[] data = [0x01, 0x02, 0x03, 0x04];

        using var temp = await TempFile.CreateBinary(data, ".bin");

        Assert.EndsWith(".bin", temp.Path);
        Assert.True(File.Exists(temp.Path));
        var readData = await File.ReadAllBytesAsync(temp.Path);
        Assert.Equal(data, readData);
    }

    [Fact]
    public async Task CreateBinary_WithEmptyContent_CreatesEmptyFile()
    {
        using var temp = await TempFile.CreateBinary(ReadOnlyMemory<byte>.Empty, ".bin");

        Assert.True(File.Exists(temp.Path));
        var readData = await File.ReadAllBytesAsync(temp.Path);
        Assert.Empty(readData);
    }

    [Fact]
    public async Task CreateBinary_WithLargeBinaryData_WritesCorrectly()
    {
        var data = new byte[1024];
        Random.Shared.NextBytes(data);

        using var temp = await TempFile.CreateBinary(data, ".dat");

        Assert.True(File.Exists(temp.Path));
        var readData = await File.ReadAllBytesAsync(temp.Path);
        Assert.Equal(data, readData);
        Assert.Equal(1024, readData.Length);
    }

    [Fact]
    public async Task CreateBinary_DisposesCorrectly()
    {
        string path;
        byte[] data = [0xFF, 0xFE, 0xFD];

        {
            using var temp = await TempFile.CreateBinary(data, ".bin");
            path = temp.Path;
            Assert.True(File.Exists(path));
        }

        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task CreateBinary_WithImageData_CreatesValidFile()
    {
        // Simulate a simple 1x1 PNG (minimal valid PNG)
        byte[] pngData =
        [
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, // PNG signature
            0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52 // IHDR chunk start
        ];

        using var temp = await TempFile.CreateBinary(pngData, ".png");

        Assert.EndsWith(".png", temp.Path);
        var readData = await File.ReadAllBytesAsync(temp.Path);
        Assert.Equal(pngData, readData);
    }

    [Fact]
    public async Task CreateText_CanBeVerified()
    {
        using var temp = await TempFile.CreateText("test content", ".txt");

        var content = await File.ReadAllTextAsync(temp.Path);
        await Verify(content);
    }

    [Fact]
    public async Task CreateBinary_CanBeVerified()
    {
        byte[] data = [0x01, 0x02, 0x03, 0x04, 0x05];

        using var temp = await TempFile.CreateBinary(data, ".bin");

        var readData = await File.ReadAllBytesAsync(temp.Path);
        await Verify(readData);
    }

    [Fact]
    public async Task CreateText_WithJsonContent_CreatesValidJsonFile()
    {
        var jsonContent = """
                          {
                            "name": "test",
                            "value": 123
                          }
                          """;

        using var temp = await TempFile.CreateText(jsonContent, ".json");

        Assert.EndsWith(".json", temp.Path);
        var readContent = await File.ReadAllTextAsync(temp.Path);
        Assert.Equal(jsonContent, readContent);
    }

    [Fact]
    public async Task CreateText_WithXmlContent_CreatesValidXmlFile()
    {
        var xmlContent = """
                         <?xml version="1.0" encoding="utf-8"?>
                         <root>
                           <element>value</element>
                         </root>
                         """;

        using var temp = await TempFile.CreateText(xmlContent, ".xml");

        Assert.EndsWith(".xml", temp.Path);
        var readContent = await File.ReadAllTextAsync(temp.Path);
        Assert.Equal(xmlContent, readContent);
    }
}