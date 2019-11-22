using System;
using System.Diagnostics;

static class DiffRunner
{
    static string verifyDiffProcess = null!;
    static string verifyDiffArgument = null!;
    public readonly static bool FoundDiff;

    static DiffRunner()
    {
        verifyDiffProcess = Environment.GetEnvironmentVariable("VerifyDiffProcess");
        verifyDiffArgument = Environment.GetEnvironmentVariable("VerifyDiffArguments");
        if (verifyDiffProcess == null && verifyDiffArgument == null)
        {
            return;
        }

        if (verifyDiffArgument == null)
        {
            throw new Exception("VerifyDiffProcess env variable found but no VerifyDiffArguments env variable found.");
        }

        if (verifyDiffProcess == null)
        {
            throw new Exception("VerifyDiffArguments env variable found but no VerifyDiffProcess env variable found.");
        }

        FoundDiff = true;
        if (!verifyDiffArgument.Contains("{receivedPath}"))
        {
            throw new Exception("Expected VerifyDiff env variable to contain '{receivedPath}'. Example: devenv /diff {receivedPath} {verifiedPath}");
        }

        if (!verifyDiffArgument.Contains("{verifiedPath}"))
        {
            throw new Exception("Expected VerifyDiff env variable to contain '{receivedPath}'. Example: devenv /diff {receivedPath} {verifiedPath}");
        }
    }

    public static void Launch(string receivedPath, string verifiedPath)
    {
        if (!FoundDiff)
        {
            return;
        }

        var replacedArguments = verifyDiffArgument
            .Replace("{receivedPath}", $"\"{receivedPath}\"")
            .Replace("{verifiedPath}", $"\"{verifiedPath}\"");
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = verifyDiffProcess,
                Arguments = replacedArguments,
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        };
        try
        {
            process.Start();
        }
        catch (Exception exception)
        {
            var message = $@"Failed to launch diff tool.
{verifyDiffProcess} {replacedArguments}";
            throw new Exception(message, exception);
        }
    }
}