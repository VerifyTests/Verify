static class FileLockKiller
{
    static FileLockKiller()
    {
        var text = Environment.GetEnvironmentVariable("Verify_KillProcessLockingFile");
        Enabled = ParseEnvironmentVariable(text);
    }

    public static bool Enabled { get; }

    public static bool ParseEnvironmentVariable(string? text)
    {
        if (text is null)
        {
            return false;
        }

        if (bool.TryParse(text, out var result))
        {
            return result;
        }

        throw new($"Could not convert `Verify_KillProcessLockingFile` environment variable to a bool. Value: {text}");
    }

    public static void KillProcessesLockingFile(string path)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        foreach (var process in RestartManager.GetProcessesLockingFile(path))
        {
            try
            {
                process.Kill();
                process.WaitForExit(5000);
            }
            catch
            {
            }
            finally
            {
                process.Dispose();
            }
        }
    }
}
