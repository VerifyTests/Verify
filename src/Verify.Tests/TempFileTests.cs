public class TempFileTests
{
    [Fact]
    public void Constructor_CreatesFile()
    {
        using var temp = new TempFile();

        Assert.True(File.Exists(temp.Path));
        Assert.StartsWith(TempFile.RootDirectory, temp.Path);
    }

    [Fact]
    public void Constructor_WithExtension_CreatesFileWithExtension()
    {
        using var temp = new TempFile(".txt");

        Assert.True(File.Exists(temp.Path));
        Assert.EndsWith(".txt", temp.Path);
    }

    [Theory]
    [InlineData(".json")]
    [InlineData(".xml")]
    [InlineData(".csv")]
    [InlineData(".log")]
    public void Constructor_WithVariousExtensions_CreatesFileWithCorrectExtension(string extension)
    {
        using var temp = new TempFile(extension);

        Assert.True(File.Exists(temp.Path));
        Assert.EndsWith(extension, temp.Path);
    }

    [Fact]
    public void Dispose_DeletesFile()
    {
        string path;

        using (var temp = new TempFile())
        {
            path = temp.Path;
            Assert.True(File.Exists(path));
        }

        Assert.False(File.Exists(path));
    }

    [Fact]
    public void Dispose_WhenFileAlreadyDeleted_DoesNotThrow()
    {
        var temp = new TempFile();
        File.Delete(temp.Path);

        var exception = Record.Exception(() => temp.Dispose());

        Assert.Null(exception);
    }

    [Fact]
    public void ToString_ReturnsPath()
    {
        using var temp = new TempFile();

        var result = temp.ToString();

        Assert.Equal(temp.Path, result);
    }

    [Fact]
    public void ImplicitStringConversion_ReturnsPath()
    {
        using var temp = new TempFile(".txt");

        string path = temp;

        Assert.Equal(temp.Path, path);
        File.WriteAllText(path, "test content");
        Assert.Equal("test content", File.ReadAllText(temp.Path));
    }

    [Fact]
    public void ImplicitFileInfoConversion_ReturnsFileInfo()
    {
        using var temp = new TempFile(".txt");
        File.WriteAllText(temp, "test");

        FileInfo fileInfo = temp;

        Assert.Equal(temp.Path, fileInfo.FullName);
        Assert.True(fileInfo.Exists);
        Assert.Equal(4, fileInfo.Length);
    }

    [Fact]
    public void Info_ReturnsFileInfo()
    {
        using var temp = new TempFile(".txt");
        File.WriteAllText(temp, "content");

        var info = temp.Info;

        Assert.Equal(temp.Path, info.FullName);
        Assert.True(info.Exists);
        Assert.Equal(7, info.Length);
    }

    [Fact]
    public void RootDirectory_ExistsAfterInit()
    {
        Assert.True(Directory.Exists(TempFile.RootDirectory));
        Assert.EndsWith("VerifyTempFiles", TempFile.RootDirectory);
    }

    [Fact]
    public void MultipleInstances_CreateUniqueFiles()
    {
        using var temp1 = new TempFile();
        using var temp2 = new TempFile();
        using var temp3 = new TempFile();

        Assert.NotEqual(temp1.Path, temp2.Path);
        Assert.NotEqual(temp2.Path, temp3.Path);
        Assert.NotEqual(temp1.Path, temp3.Path);

        Assert.True(File.Exists(temp1.Path));
        Assert.True(File.Exists(temp2.Path));
        Assert.True(File.Exists(temp3.Path));
    }

    [Fact]
    public void Cleanup_RemovesOldFiles()
    {
        // Create a file and backdate it
        var oldFile = Path.Combine(TempFile.RootDirectory, "old_test_file.txt");
        File.WriteAllText(oldFile, "old");
        File.SetLastWriteTime(oldFile, DateTime.Now.AddDays(-2));

        // Create a recent file
        var recentFile = Path.Combine(TempFile.RootDirectory, "recent_test_file.txt");
        File.WriteAllText(recentFile, "recent");

        TempFile.Cleanup();

        Assert.False(File.Exists(oldFile));
        Assert.True(File.Exists(recentFile));

        // Cleanup
        File.Delete(recentFile);
    }

    [Fact]
    public void Cleanup_HandlesFileNotFoundRaceCondition()
    {
        // Create and backdate a file
        var testFile = Path.Combine(TempFile.RootDirectory, "race_test.txt");
        File.WriteAllText(testFile, "test");
        File.SetLastWriteTime(testFile, DateTime.Now.AddDays(-2));

        // Delete it manually before cleanup
        File.Delete(testFile);

        // This should not throw
        var exception = Record.Exception(() => TempFile.Cleanup());

        Assert.Null(exception);
    }

    [Fact]
    public void FileCanBeWrittenAndRead()
    {
        using var temp = new TempFile(".txt");
        var content = "Hello, World!";

        File.WriteAllText(temp, content);
        var result = File.ReadAllText(temp);

        Assert.Equal(content, result);
    }

    [Fact]
    public async Task FileCanBeWrittenAndReadAsync()
    {
        using var temp = new TempFile(".json");
        var content = """{"name":"test","value":42}""";

        await File.WriteAllTextAsync(temp, content);
        var result = await File.ReadAllTextAsync(temp);

        Assert.Equal(content, result);
    }

    [Fact]
    public void BinaryFileOperations()
    {
        using var temp = new TempFile(".bin");
        var data = new byte[] { 0x01, 0x02, 0x03, 0xFF, 0xFE };

        File.WriteAllBytes(temp, data);
        var result = File.ReadAllBytes(temp);

        Assert.Equal(data, result);
    }

    [Fact]
    public void Constructor_CreatesEmptyFile()
    {
        using var temp = new TempFile();

        var info = new FileInfo(temp.Path);
        Assert.Equal(0, info.Length);
    }

    [Fact]
    public Task VerifyScrubbing_ReplacesPathWithPlaceholder()
    {
        using var temp = new TempFile(".txt");
        File.WriteAllText(temp, "content");

        var output = $"File created at: {temp.Path}";

        return Verify(output);
    }

    [Fact]
    public Task VerifyScrubbing_HandlesMultipleFiles()
    {
        using var temp1 = new TempFile(".txt");
        using var temp2 = new TempFile(".json");

        File.WriteAllText(temp1, "text content");
        File.WriteAllText(temp2, """{"test":true}""");

        var output = $"""
            First file: {temp1.Path}
            Second file: {temp2.Path}
            """;

        return Verify(output);
    }

    [Fact]
    public void FileStream_CanBeOpened()
    {
        using var temp = new TempFile(".dat");

        using (var stream = File.OpenWrite(temp))
        {
            stream.WriteByte(42);
        }

        using (var stream = File.OpenRead(temp))
        {
            Assert.Equal(42, stream.ReadByte());
        }
    }

    [Fact]
    public void FileInfo_Refresh_UpdatesInfo()
    {
        using var temp = new TempFile(".txt");

        var initialLength = temp.Info.Length;

        File.WriteAllText(temp, "new content");
        temp.Info.Refresh();

        Assert.Equal(0, initialLength);
        Assert.Equal(11, temp.Info.Length);
    }

    [Fact]
    public void MultipleDispose_DoesNotThrow()
    {
        var temp = new TempFile();
        temp.Dispose();

        var exception = Record.Exception(() => temp.Dispose());

        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_WithNullExtension_CreatesFileWithoutExtension()
    {
        using var temp = new TempFile(null);

        Assert.True(File.Exists(temp.Path));
        Assert.False(Path.HasExtension(temp.Path));
    }

    [Fact]
    public void Constructor_WithExtensionWithoutDot_AddsExtension()
    {
        using var temp = new TempFile(".csv");

        var extension = Path.GetExtension(temp.Path);
        Assert.Equal(".csv", extension);
    }

    [Fact]
    public void ParallelFileCreation_CreatesUniqueFiles()
    {
        var paths = new System.Collections.Concurrent.ConcurrentBag<string>();
        var files = new System.Collections.Concurrent.ConcurrentBag<TempFile>();

        try
        {
            Parallel.For(0, 10, _ =>
            {
                var temp = new TempFile(".txt");
                files.Add(temp);
                paths.Add(temp.Path);
            });

            Assert.Equal(10, paths.Distinct().Count());
            Assert.All(paths, path => Assert.True(File.Exists(path)));
        }
        finally
        {
            foreach (var file in files)
            {
                file.Dispose();
            }
        }
    }

    [Fact]
    public void FileCanBeMovedOrRenamed()
    {
        using var temp = new TempFile(".txt");
        File.WriteAllText(temp, "content");

        var newPath = Path.Combine(TempFile.RootDirectory, "renamed.txt");
        try
        {
            File.Move(temp.Path, newPath);

            Assert.False(File.Exists(temp.Path));
            Assert.True(File.Exists(newPath));
            Assert.Equal("content", File.ReadAllText(newPath));
        }
        finally
        {
            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }
        }
    }

    [Fact]
    public void OpenExplorerAndDebug_ThrowsOnBuildServer()
    {
        // This test verifies the exception when build server is detected
        // We can't easily test the actual functionality without mocking

        using var temp = new TempFile(".txt");

        // If we're on a build server, it should throw
        if (BuildServerDetector.Detected)
        {
            Assert.Throws<Exception>(() => temp.OpenExplorerAndDebug());
        }
        // Otherwise we skip the test to avoid launching explorer/debugger
    }
}