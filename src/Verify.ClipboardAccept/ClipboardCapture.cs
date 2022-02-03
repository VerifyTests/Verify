﻿using System.Runtime.InteropServices;
using DiffEngine;
using TextCopy;

static class ClipboardCapture
{
    static StringBuilder builder = new();
    static SemaphoreSlim semaphore = new(1, 1);

    static string moveCommand;
    static string deleteCommand;

    public static void Enable()
    {
        throw new NotImplementedException();
    }

    static ClipboardCapture()
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

    public static void Clear()
    {
        builder = new();
    }

    public static string Read()
    {
        return builder.ToString();
    }

    public static Task AppendMove(string received, string verified)
    {
        return Append(string.Format(moveCommand, received, verified));
    }

    public static Task AppendDelete(string verified)
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