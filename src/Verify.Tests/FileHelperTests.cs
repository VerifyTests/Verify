public class FileHelperTests
{
    [Fact]
    public void ShouldNotLock()
    {
        using (IoHelpers.OpenRead(ProjectFiles.sample_txt))
        {
            Assert.False(FileEx.IsFileReadLocked(ProjectFiles.sample_txt));
        }
    }
}