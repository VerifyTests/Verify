using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class FileComparerTests :
    VerifyBase
{
    [Fact]
    public async Task BinaryEquals()
    {
        File.Copy("sample.bmp", "sample.tmp", true);
        try
        {
            Assert.True(await FileComparer.DefaultCompare("sample.bmp", "sample.tmp"));
        }
        finally
        {
            File.Delete("sample.tmp");
        }
    }
    [Fact]
    public async Task BinaryNotEqualsSameLength()
    {
        File.Copy("sample.bmp", "sample.tmp", true);
        using (var stream = File.Open("sample.tmp", FileMode.Open))
        {
            stream.Position = 100;
            stream.WriteByte(8);
        }

        try
        {
            Assert.False(await FileComparer.DefaultCompare("sample.bmp", "sample.tmp"));
        }
        finally
        {
            File.Delete("sample.tmp");
        }
    }

    [Fact]
    public async Task BinaryNotEquals()
    {
        Assert.False(await FileComparer.DefaultCompare("sample.bmp", "sample.txt"));
    }

    [Fact]
    public async Task ShouldNotLock()
    {
        File.Copy("sample.bmp", "sample.tmp", true);
        try
        {
            using (new FileStream("sample.bmp",
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            {
                Assert.True(await FileComparer.DefaultCompare("sample.bmp", "sample.tmp"));
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