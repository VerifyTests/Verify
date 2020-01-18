using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

static class ProcessCleanup
{
    static List<ProcessCommand> processCommands;

    static ProcessCleanup()
    {
        if (!BuildServerDetector.Detected &&
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            processCommands = FindAll().ToList();
        }
        else
        {
            processCommands = new List<ProcessCommand>();
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern SafeProcessHandle OpenProcess(
        int access,
        bool inherit,
        int processId);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern bool TerminateProcess(
        SafeProcessHandle processHandle,
        int exitCode);

    public static void Kill(string command)
    {
        foreach (var processCommand in processCommands
            .Where(x => x.Command == command))
        {
            TerminalProcessIfExists(processCommand);
        }
    }

    static void TerminalProcessIfExists(ProcessCommand processCommand)
    {
        var processId = (int) processCommand.Process;
        using var processHandle = OpenProcess(4097, false, processId);
        if (processHandle.IsInvalid)
        {
            return;
        }

        TerminateProcess(processHandle, -1);
    }

    public static IEnumerable<ProcessCommand> FindAll()
    {
        var wmiQuery = @"
select CommandLine, ProcessId
from Win32_Process
where CommandLine like '%.received.%'";
        using var searcher = new ManagementObjectSearcher(wmiQuery);
        using var collection = searcher.Get();
        foreach (var process in collection)
        {
            var command = (string) process["CommandLine"];
            var id = (uint) process["ProcessId"];
            process.Dispose();
            yield return new ProcessCommand(command, id);
        }
    }
}