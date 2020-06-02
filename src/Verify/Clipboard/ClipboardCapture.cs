using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextCopy;

static class ClipboardCapture
{
    static StringBuilder builder = new StringBuilder();
    static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    static string moveCommand;
    static string deleteCommand;

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

        var envMoveCommand = Environment.GetEnvironmentVariable("Verify.MoveCommand");
        if (envMoveCommand != null)
        {
            moveCommand = envMoveCommand;
        }

        var envDeleteCommand = Environment.GetEnvironmentVariable("Verify.DeleteCommand");
        if (envDeleteCommand != null)
        {
            deleteCommand = envDeleteCommand;
        }
    }

    public static void Clear()
    {
        builder = new StringBuilder();
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