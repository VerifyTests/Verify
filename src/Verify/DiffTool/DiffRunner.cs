using System.Diagnostics;

static class DiffRunner
{
    public static void Launch(ResolvedDiffTool diffTool, string receivedPath, string verifiedPath)
    {
        var arguments = Arguments(diffTool, receivedPath, verifiedPath);
        ProcessCleanup.Kill($"\"{diffTool.ExePath}\" {arguments}");
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

    static string Arguments(ResolvedDiffTool diffTool, string receivedPath, string verifiedPath)
    {
        if (diffTool.ArgumentPrefix == null)
        {
            return $"\"{receivedPath}\" \"{verifiedPath}\"";
        }
        return $"{diffTool.ArgumentPrefix} \"{receivedPath}\" \"{verifiedPath}\"";
    }
}