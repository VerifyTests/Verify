public class IoHelpersTests
{
    [Theory]
    [InlineData(@"C:\test\my_file.cs", @"C:\test")]
    [InlineData("C:/test/sub_folder/my_file.cs", "C:/test/sub_folder")]
    [InlineData("/mnt/d/test/my_file.cs", "/mnt/d/test")]
    [InlineData("/mnt/MyFile.cs", "/mnt")]
    [InlineData(@"C:\Program Files/MyFile.cs", @"C:\Program Files")]
    [InlineData(@"C:\MyFile.cs", "C:")]
    public void ResolveDirectoryNameFromSourceFileTests(string sourceFile, string expectedDirectory)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
            expectedDirectory != "C:")
        {
            var directory = Path.GetDirectoryName(sourceFile) ?? "";
            Assert.Equal(expectedDirectory.Replace('/', '\\'), directory);
        }

        Assert.Equal(expectedDirectory, IoHelpers.ResolveDirectoryFromSourceFile(sourceFile));
    }

    [Fact]
    public async Task WriteStreamHandlesExclusiveFileStream()
    {
        using var source = new TempFile(".bin");
        await File.WriteAllBytesAsync(source, [1, 2, 3, 4, 5, 6, 7, 8]);
        // The constructor only reserves the path, so WriteStream is what creates the file
        using var destination = new TempFile(".bin");

        // Writable + FileShare.None means File.Copy cannot re-open the source
        // by path on Windows; WriteStream must fall back to the handle.
        using var stream = new FileStream(source, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        await IoHelpers.WriteStream(destination, stream);

        Assert.True(File.Exists(destination));
        Assert.Equal(8, destination.Info.Length);
    }

    [Fact]
    public void ExtensionForExtensionlessFileStream()
    {
        using var directory = new TempDirectory();
        var path = directory.BuildPath("Dockerfile");
        File.WriteAllText(path, "content");

        using var stream = File.OpenRead(path);
        Assert.Equal("noextension", stream.Extension());
    }
}