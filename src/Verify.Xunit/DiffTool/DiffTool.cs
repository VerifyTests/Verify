using System;
using System.IO;

class DiffTool
{
    public string Name { get; }
    public string Url { get; }
    public string ArgumentPrefix { get; }
    public string[] BinaryExtensions { get; }
    public string? ExePath { get; }
    public bool Exists { get; }

    public DiffTool(string name, string url, string argumentPrefix, string[] exePaths, string[] binaryExtensions)
    {
        Name = name;
        Url = url;
        ArgumentPrefix = argumentPrefix;
        BinaryExtensions = binaryExtensions;

        foreach (var exePath in exePaths)
        {
            var expanded = Environment.ExpandEnvironmentVariables(exePath);
            if (File.Exists(expanded))
            {
                ExePath = expanded;
                Exists = true;
            }
        }
    }
}