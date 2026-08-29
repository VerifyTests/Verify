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

    // RmGetList reports the caller too when the lock is held in process. Killing that
    // would kill the test run, so the current process must never be returned.
    [Fact]
    public void CurrentProcessIsNotReported()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        using var file = new TempFile(".txt");
        File.WriteAllText(file, "content");

        // ReSharper disable once UseAwaitUsing
        using var locker = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

        var processes = RestartManager.GetProcessesLockingFile(file);
        try
        {
            using var current = Process.GetCurrentProcess();
            Assert.DoesNotContain(processes, _ => _.Id == current.Id);
        }
        finally
        {
            foreach (var process in processes)
            {
                process.Dispose();
            }
        }
    }
}
