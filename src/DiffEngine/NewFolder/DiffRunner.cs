using System.Diagnostics;
using DiffEngine;

public static class DiffRunner
{
    public static void KillProcessIfSupported(ResolvedDiffTool tool, string path1, string path2)
    {
        var command = tool.BuildCommand(path1, path2);

        if (tool.IsMdi)
        {
            return;
        }

        ProcessCleanup.Kill(command);
    }

    public static void Launch(ResolvedDiffTool tool, string path1, string path2)
    {
        var command = tool.BuildCommand(path1, path2);
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
            }
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = tool.ExePath,
                Arguments = tool.BuildArguments(path1, path2),
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };
        process.StartWithCatch();
    }
}