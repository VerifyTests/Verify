using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class FileHelperTests :
    VerifyBase
{
    [Fact]
    public void ShouldNotLock()
    {
        using (FileHelpers.OpenRead("sample.txt"))
        {
            Assert.False(FileEx.IsFileReadLocked("sample.txt"));
        }
    }

    public FileHelperTests(ITestOutputHelper output) :
        base(output)
    {
    }
}