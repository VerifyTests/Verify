public class FileLockKillerTests
{
    [Fact]
    public void ParseEnvironmentVariable()
    {
        Assert.False(FileLockKiller.ParseEnvironmentVariable(null));
        Assert.False(FileLockKiller.ParseEnvironmentVariable("false"));
        Assert.True(FileLockKiller.ParseEnvironmentVariable("true"));
    }

    [Fact]
    public Task ParseEnvironmentVariable_failure() =>
        Throws(() => FileLockKiller.ParseEnvironmentVariable("foo"));
}
