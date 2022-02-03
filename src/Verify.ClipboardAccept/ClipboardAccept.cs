using System.Runtime.InteropServices;
using DiffEngine;
using TextCopy;

namespace VerifyTests;

public static class ClipboardAccept
{
    static StringBuilder builder = new();
    static SemaphoreSlim semaphore = new(1, 1);

    static string moveCommand;
    static string deleteCommand;

    public static void Enable()
    {
        VerifierSettings.OnFirstVerify(AppendMove);
        VerifierSettings.OnDelete(AppendDelete);
        VerifierSettings.OnVerifyMismatch((file, _) => AppendMove(file));
    }

    static Task AppendMove(FilePair file)
    {
        return Append(string.Format(moveCommand, file.ReceivedPath, file.VerifiedPath));
    }

    static ClipboardAccept()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            moveCommand = "cmd /c move /Y \"{0}\" \"{1}\"";
            deleteCommand = "cmd /c del \"{0}\"";
        }
        else
        {
            moveCommand = "mv -f \"{0}\" \"{1}\"";
            deleteCommand = "rm -f \"{0}\"";
        }

        var envMoveCommand = Environment.GetEnvironmentVariable("Verify_MoveCommand");
        if (envMoveCommand is not null)
        {
            moveCommand = envMoveCommand;
        }

        var envDeleteCommand = Environment.GetEnvironmentVariable("Verify_DeleteCommand");
        if (envDeleteCommand is not null)
        {
            deleteCommand = envDeleteCommand;
        }
    }

    internal static void Clear()
    {
        builder = new();
    }

    internal static string Read()
    {
        return builder.ToString();
    }

    internal static Task AppendDelete(string verified)
    {
        return Append(string.Format(deleteCommand, verified));
    }

    static async Task Append(string command)
    {
        if (!ClipboardEnabled.IsEnabled())
        {
            return;
        }

        if (DiffEngineTray.IsRunning)
        {
            return;
        }

        await semaphore.WaitAsync();

        try
        {
            builder.AppendLine(command);
            await ClipboardService.SetTextAsync(builder.ToString());
        }
        finally
        {
            semaphore.Release();
        }
    }
}