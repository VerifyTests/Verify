using System.Runtime.InteropServices;

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
}