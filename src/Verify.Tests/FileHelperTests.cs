public class FileHelperTests
{
    [Fact]
    public void ShouldNotLock()
    {
        using (IoHelpers.OpenRead("sample.txt"))
        {
            Assert.False(FileEx.IsFileReadLocked("sample.txt"));
        }
    }
}