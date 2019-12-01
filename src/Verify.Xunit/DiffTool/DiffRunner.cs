using System.Diagnostics;

static class DiffRunner
{
    internal static bool Enabled = true;

    public static void Launch(ResolvedDiffTool diffTool, string receivedPath, string verifiedPath)
    {
        if (!Enabled)
        {
            return;
        }
        var arguments = $"{diffTool.ArgumentPrefix} \"{receivedPath}\" \"{verifiedPath}\"";
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
}