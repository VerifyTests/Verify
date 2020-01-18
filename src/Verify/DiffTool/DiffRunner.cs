using System.Diagnostics;

static class DiffRunner
{
    public static void Launch(ResolvedDiffTool tool, FilePair filePair)
    {
        var arguments = tool.BuildArguments(filePair);
        if (tool.ShouldTerminate)
        {
            ProcessCleanup.Kill($"\"{tool.ExePath}\" {arguments}");
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = tool.ExePath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };
        process.StartWithCatch();
    }
}