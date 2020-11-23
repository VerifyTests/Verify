using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextCopy;

static class ClipboardCapture
{
    static StringBuilder builder = new();
    static SemaphoreSlim semaphore = new(1, 1);

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

        var envMoveCommand = EnvironmentEx.GetEnvironmentVariable("Verify_MoveCommand");
        if (envMoveCommand != null)
        {
            moveCommand = envMoveCommand;
        }

        var envDeleteCommand = EnvironmentEx.GetEnvironmentVariable("Verify_DeleteCommand");
        if (envDeleteCommand != null)
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