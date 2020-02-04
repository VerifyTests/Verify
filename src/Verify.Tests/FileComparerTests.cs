using System.IO;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class FileComparerTests :
    VerifyBase
{
    [Fact]
    public void SamePathEquals()
    {
        Assert.True(FileComparer.FilesEqual("sample.bmp", "sample.bmp"));
    }

    [Fact]
    public void Equals()
    {
        try
        {
            File.Copy("sample.bmp", "sample.tmp", true);
            Assert.True(FileComparer.FilesEqual("sample.bmp", "sample.tmp"));
        }
        finally
        {
            File.Delete("sample.tmp");
        }
    }

    [Fact]
    public void NotEquals()
    {
        Assert.False(FileComparer.FilesEqual("sample.bmp", "sample.txt"));
    }

    [Fact]
    public void ShouldNotLock()
    {
        try
        {
            File.Copy("sample.bmp", "sample.tmp", true);
            using (new FileStream("sample.bmp",
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                Assert.True(FileComparer.FilesEqual("sample.bmp", "sample.tmp"));
            }
        }
        finally
        {
            File.Delete("sample.tmp");
        }
    }

    public FileComparerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}