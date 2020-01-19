using System.Diagnostics;

static class DiffRunner
{
    public static void KillProcessIfSupported(ResolvedDiffTool tool, FilePair filePair)
    {
        var command = tool.BuildCommand(filePair);

        if (tool.IsMdi)
        {
            return;
        }

        ProcessCleanup.Kill(command);
    }

    public static void Launch(ResolvedDiffTool tool, FilePair filePair)
    {
        var command = tool.BuildCommand(filePair);
        var isDiffToolRunning = ProcessCleanup.IsRunning(command);
        if (isDiffToolRunning)
        {
            if (tool.SupportsAutoRefresh)
            {
                return;
            }
            if (!tool.IsMdi)
            {
                ProcessCleanup.Kill(command);
                return;
            }
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = tool.ExePath,
                Arguments = tool.BuildArguments(filePair),
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };
        process.StartWithCatch();
    }
}