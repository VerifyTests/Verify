using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DiffEngine;

class ToolDefinition
{
    public DiffTool Name { get; }
    public string Url { get; }
    public bool SupportsAutoRefresh { get; }
    public bool IsMdi { get; }
    public Func<string, string, string> BuildWindowsArguments { get; }
    public Func<string, string, string> BuildLinuxArguments { get; }
    public Func<string, string, string> BuildOsxArguments { get; }

    public Func<string, string, string> BuildArguments
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return BuildWindowsArguments;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return BuildLinuxArguments;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return BuildOsxArguments;
            }

            throw new Exception($"OS not supported: {RuntimeInformation.OSDescription}");
        }
    }

    public string[] BinaryExtensions { get; }
    public string? ExePath { get; private set; }
    public bool Exists { get; private set; }
    public string[] WindowsExePaths { get; }
    public string[] LinuxExePaths { get; }
    public string[] OsxExePaths { get; }
    public string? Notes { get; }
    public bool SupportsText { get; }

    public ToolDefinition(
        DiffTool name,
        string url,
        bool supportsAutoRefresh,
        bool isMdi,
        bool supportsText,
        Func<string, string, string> buildArguments,
        string[] windowsExePaths,
        string[] binaryExtensions,
        string[] linuxExePaths,
        string[] osxExePaths,
        string? notes = null)
    {
        Name = name;
        Url = url;
        SupportsAutoRefresh = supportsAutoRefresh;
        IsMdi = isMdi;
        BuildWindowsArguments = buildArguments;
        BuildLinuxArguments = buildArguments;
        BuildOsxArguments = buildArguments;
        BinaryExtensions = binaryExtensions;
        WindowsExePaths = windowsExePaths;
        LinuxExePaths = linuxExePaths;
        OsxExePaths = osxExePaths;
        Notes = notes;
        SupportsText = supportsText;

        FindExe();
    }

    void FindExe()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            FindExe(WindowsExePaths);
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            FindExe(LinuxExePaths);
            return;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            FindExe(OsxExePaths);
            return;
        }

        throw new Exception($"OS not supported: {RuntimeInformation.OSDescription}");
    }

    void FindExe(string[] exePaths)
    {
        foreach (var exePath in exePaths)
        {
            if (WildcardFileFinder.TryFind(exePath, out var result))
            {
                ExePath = result;
                Exists = true;
                return;
            }
        }
    }

}