using System.IO;
using System.Threading.Tasks;
using VerifyTesting;
using Xunit;

public class FileComparerTests
{
    [Fact]
    public async Task BinaryEquals()
    {
        File.Copy("sample.bmp", "sample.tmp", true);
        try
        {
            var result = await FileComparer.DefaultCompare(VerifySettings.Default, "sample.bmp", "sample.tmp");
            Assert.True(result.IsEqual);
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
            var result = await FileComparer.DefaultCompare(VerifySettings.Default, "sample.bmp", "sample.tmp");
            Assert.False(result.IsEqual);
        }
        finally
        {
            File.Delete("sample.tmp");
        }
    }

    [Fact]
    public async Task BinaryNotEquals()
    {
        var result = await FileComparer.DefaultCompare(VerifySettings.Default, "sample.bmp", "sample.txt");
        Assert.False(result.IsEqual);
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
                var result = await FileComparer.DefaultCompare(VerifySettings.Default, "sample.bmp", "sample.tmp");
                Assert.True(result.IsEqual);
            }
        }
        finally
        {
            File.Delete("sample.tmp");
        }
    }
}