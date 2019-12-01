using System;
using System.Diagnostics;

static class DiffRunner
    {
    static string verifyDiffCommand = null!;
    public readonly static bool FoundDiff;

    static DiffRunner()
    {
        verifyDiffCommand = Environment.GetEnvironmentVariable("VerifyDiffCommand");
        if (verifyDiffCommand == null)
        {
            return;
        }

        FoundDiff = true;
        if (!verifyDiffCommand.Contains("{receivedPath}"))
        {
            throw new Exception("Expected VerifyDiff env variable to contain '{receivedPath}'. Example: devenv /diff {receivedPath} {verifiedPath}");
        }

        if (!verifyDiffCommand.Contains("{verifiedPath}"))
        {
            throw new Exception("Expected VerifyDiff env variable to contain '{receivedPath}'. Example: devenv /diff {receivedPath} {verifiedPath}");
        }
    }

    internal static bool Enabled = true;

    public static void Launch(string receivedPath, string verifiedPath)
    {
        if (!FoundDiff)
        {
            return;
        }

        if (!Enabled)
        {
            return;
        }

        var replaced = verifyDiffCommand
            .Replace("{receivedPath}", $"\"{receivedPath}\"")
            .Replace("{verifiedPath}", $"\"{verifiedPath}\"");
        var arguments = $"/K \"{replaced}\"";
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };
        process.StartWithCatch();
    }

    public static void Launch(string exe, string argumentPrefix, string receivedPath, string verifiedPath)
    {
        var arguments = $"{argumentPrefix} \"{receivedPath}\" \"{verifiedPath}\"";
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exe,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };
        process.StartWithCatch();
    }
    public static void Launch(ResolvedDiffTool diffTool, string receivedPath, string verifiedPath)
    {
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