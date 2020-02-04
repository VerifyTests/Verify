using System.IO;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class FileComparerTests :
    VerifyBase
{
    [Fact]
    public void Equals()
    {
        Assert.True(FileComparer.FilesEqual("sample.bmp", "sample.bmp"));
    }

    [Fact]
    public void NotEquals()
    {
        Assert.False(FileComparer.FilesEqual("sample.bmp", "sample.txt"));
    }
#if NETCOREAPP3_1

    [Fact]
    public void ShouldNotLock()
    {
        using (File.OpenWrite("sample.bmp"))
        {
            Assert.True(FileComparer.FilesEqual("sample.bmp", "sample.bmp"));
        }
    }

#endif

    public FileComparerTests(ITestOutputHelper output) :
        base(output)
    {
    }
}