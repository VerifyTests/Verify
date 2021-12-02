public class FileHelperTests
{
    [Fact]
    public void ShouldNotLock()
    {
        using (FileHelpers.OpenRead("sample.txt"))
        {
            Assert.False(FileEx.IsFileReadLocked("sample.txt"));
        }
    }
}