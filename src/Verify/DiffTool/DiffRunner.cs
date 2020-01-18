using System.Diagnostics;

static class DiffRunner
{
    public static void Launch(ResolvedDiffTool diffTool, FilePair filePair)
    {
        var arguments = Arguments(diffTool, filePair);
        if (diffTool.ShouldTerminate)
        {
            ProcessCleanup.Kill($"\"{diffTool.ExePath}\" {arguments}");
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = diffTool.ExePath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };
        process.StartWithCatch();
    }

    static string Arguments(ResolvedDiffTool diffTool, FilePair filePair)
    {
        if (diffTool.ArgumentPrefix == null)
        {
            return $"\"{filePair.Received}\" \"{filePair.Verified}\"";
        }
        return $"{diffTool.ArgumentPrefix} \"{filePair.Received}\" \"{filePair.Verified}\"";
    }
}