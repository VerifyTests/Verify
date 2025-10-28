public class TempDirectoryTests
{
    // [Fact]
    // public Task VerifyInstance()
    // {
    //     using var directory = new TempDirectory();
    //     File.WriteAllText(Path.Combine(directory, "test.txt"), "test");
    //     return Verify(directory);
    // }
    //
    // [Fact]
    // public Task VerifyInstanceNested()
    // {
    //     using var directory = new TempDirectory();
    //     File.WriteAllText(Path.Combine(directory, "test.txt"), "test");
    //     return Verify(
    //         new
    //         {
    //             directory
    //         });
    // }

    #region VerifyTempDirectory

    [Fact]
    public async Task VerifyDirectoryInstance()
    {
        using var directory = new TempDirectory();
        await File.WriteAllTextAsync(Path.Combine(directory, "test.txt"), "test");
        await VerifyDirectory(directory);
    }

    #endregion

    #region TempDirectory

    [Fact]
    public void Usage()
    {
        using var temp = new TempDirectory();

        // write a file to the temp directory
        File.WriteAllText(Path.Combine(temp, "test.txt"), "content");

        // Directory and files automatically deleted here
    }

    #endregion

    #region TempDirectoryStringConversion

    [Fact]
    public void StringConversion()
    {
        using var temp = new TempDirectory();

        File.WriteAllText(Path.Combine(temp, "test.txt"), "content");

        // implicit conversion to string
        string path = temp;
        var files = Directory.EnumerateFiles(path);
        Trace.WriteLine(files.Count());
    }

    #endregion

    #region TempDirectoryDirectoryInfoConversion

    [Fact]
    public void DirectoryInfoConversion()
    {
        using var temp = new TempDirectory();

        File.WriteAllText(Path.Combine(temp, "test.txt"), "content");

        // implicit conversion to DirectoryInfo
        DirectoryInfo info = temp;
        var files = info.EnumerateFiles();
        Trace.WriteLine(files.Count());
    }

    #endregion

    #region TempDirectoryInfoProperty

    [Fact]
    public void InfoProperty()
    {
        using var temp = new TempDirectory();

        File.WriteAllText(Path.Combine(temp, "test.txt"), "content");

        var files = temp.Info.EnumerateFiles();
        Trace.WriteLine(files.Count());

        temp.Info.CreateSubdirectory("Subdirectory");
    }

    #endregion

    #region TempDirectoryRootDirectory

    [Fact]
    public void RootDirectory() =>
        // Accessing the root directory for all TempDirectory instances
        Trace.WriteLine(TempDirectory.RootDirectory);

    #endregion

    [Fact]
    public void CreatesDirectory()
    {
        using var directory = new TempDirectory();
        string path = directory;

        Assert.True(Directory.Exists(path));
    }

    [Fact]
    public void CreatesUniqueDirectories()
    {
        using var dir1 = new TempDirectory();
        using var dir2 = new TempDirectory();

        string path1 = dir1;
        string path2 = dir2;

        Assert.NotEqual(path1, path2);
        Assert.True(Directory.Exists(path1));
        Assert.True(Directory.Exists(path2));
    }

    [Fact]
    public void Dispose_DeletesDirectory()
    {
        string path;

        using (var directory = new TempDirectory())
        {
            path = directory;
            Assert.True(Directory.Exists(directory));
        }

        Assert.False(Directory.Exists(path));
    }

    [Fact]
    public void Dispose_DeletesDirectoryWithContents()
    {
        string path;

        using (var directory = new TempDirectory())
        {
            path = directory;

            // Create some files and subdirectories
            File.WriteAllText(Path.Combine(directory, "test.txt"), "content");
            Directory.CreateDirectory(Path.Combine(path, "subdir"));
            File.WriteAllText(Path.Combine(directory, "subdir", "nested.txt"), "nested");

            Assert.True(Directory.Exists(directory));
        }

        Assert.False(Directory.Exists(path));
    }

    [Fact]
    public void ImplicitStringConversion_ReturnsDirectoryPath()
    {
        using var directory = new TempDirectory();
        Assert.False(string.IsNullOrEmpty(directory));
        Assert.True(Directory.Exists(directory));
    }

    [Fact]
    public void DeleteOldDirectories()
    {
        // Create an old directory
        var oldDirPath = Path.Combine(TempDirectory.RootDirectory, "OldDirectory");
        Directory.CreateDirectory(oldDirPath);

        // Set LastWriteTime to 2 days ago
        Directory.SetLastWriteTime(oldDirPath, DateTime.Now.AddDays(-2));

        // Create a recent directory
        var recentDirPath = Path.Combine(TempDirectory.RootDirectory, "RecentDirectory");
        Directory.CreateDirectory(recentDirPath);
        Directory.SetLastWriteTime(recentDirPath, DateTime.Now.AddHours(-12));

        TempDirectory.Cleanup();

        Assert.False(Directory.Exists(oldDirPath));
        Assert.True(Directory.Exists(recentDirPath));
    }

    [Fact]
    public void DeleteOldDirectoriesWithContents()
    {
        var oldDirPath = Path.Combine(TempDirectory.RootDirectory, "OldDirectoryWithFiles");
        Directory.CreateDirectory(oldDirPath);

        // Add contents
        File.WriteAllText(Path.Combine(oldDirPath, "file.txt"), "content");
        var subDir = Path.Combine(oldDirPath, "subdir");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "nested.txt"), "nested");

        // Make it old
        Directory.SetLastWriteTime(oldDirPath, DateTime.Now.AddDays(-2));

        TempDirectory.Cleanup();

        Assert.False(Directory.Exists(oldDirPath));
    }

    [Fact]
    public void MultipleInstances_CanCoexist()
    {
        using var dir1 = new TempDirectory();
        using var dir2 = new TempDirectory();
        using var dir3 = new TempDirectory();

        string path1 = dir1;
        string path2 = dir2;
        string path3 = dir3;

        Assert.All([path1, path2, path3], path => Assert.True(Directory.Exists(path)));
    }

    #region TempDirectoryPathProperty

    [Fact]
    public void PathProperty()
    {
        using var temp = new TempDirectory();
        var path = temp.Path;
        Assert.True(Directory.Exists(path));
        Assert.True(Path.IsPathRooted(path));
    }

    #endregion

    [Fact]
    public void DeleteDirectory()
    {
        using var temp = new TempDirectory();
        Directory.Delete(temp);
        Assert.False(Directory.Exists(temp));
    }

    [Fact]
    public void ToStringOverride()
    {
        using var temp = new TempDirectory();
        Assert.Equal(temp.Path, temp.ToString());
    }

    #region TempDirectoryOpenExplorerAndDebug

    [Fact]
    public void OpenExplorerAndDebug()
    {
        using var temp = new TempDirectory();

        File.WriteAllText(Path.Combine(temp, "file.txt"), "content");

        temp.OpenExplorerAndDebug();
    }

    #endregion

    #region TempDirectoryNoUsing

    [Fact(Explicit = true)]
    public void NoUsing()
    {
        var temp = new TempDirectory();

        File.WriteAllText(Path.Combine(temp, "file.txt"), "content");

        temp.OpenExplorerAndDebug();
    }

    #endregion

    // [Fact]
    // public void LockedFile()
    // {
    //     using var temp = new TempDirectory();
    //     File.Create(Path.Combine(temp, "test.txt"));
    // }
}