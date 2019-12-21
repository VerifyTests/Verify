using System;
using System.IO;

class DiffTool
{
    public string Name { get; }
    public string Url { get; }
    public string ArgumentPrefix { get; }
    public string[] BinaryExtensions { get; }
    public string? ExePath { get; private set; }
    public bool Exists { get; private set; }
    public string[] WindowsExePaths { get; }

    public DiffTool(
        string name,
        string url,
        string argumentPrefix,
        string[] windowsExePaths,
        string[] binaryExtensions)
    {
        Name = name;
        Url = url;
        ArgumentPrefix = argumentPrefix;
        BinaryExtensions = binaryExtensions;
        WindowsExePaths = windowsExePaths;

        FindExe(WindowsExePaths);
    }

    void FindExe(string[] exePaths)
    {
        foreach (var exePath in exePaths)
        {
            var expanded = Environment.ExpandEnvironmentVariables(exePath);
            if (!File.Exists(expanded))
            {
                continue;
            }
            ExePath = expanded;
            Exists = true;
            return;
        }
    }
}