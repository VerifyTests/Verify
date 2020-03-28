using System;
using System.Collections.Generic;
using DiffEngine;

class ResolvedDiffTool
{
    public DiffTool Name { get; }
    public string ExePath { get; }
    public Func<string, string, string> BuildArguments { get; }
    public bool IsMdi { get; }
    public bool SupportsAutoRefresh { get; }
    public IReadOnlyList<string> BinaryExtensions { get; }

    public string BuildCommand(string path1, string path2)
    {
        return $"\"{ExePath}\" {BuildArguments(path1, path2)}";
    }

    public ResolvedDiffTool(DiffTool name,
        string exePath,
        Func<string, string, string> buildArguments,
        bool isMdi,
        bool supportsAutoRefresh,
        string[] binaryExtensions)
    {
        Name = name;
        ExePath = exePath;
        BuildArguments = buildArguments;
        IsMdi = isMdi;
        SupportsAutoRefresh = supportsAutoRefresh;
        BinaryExtensions = binaryExtensions;
    }
}