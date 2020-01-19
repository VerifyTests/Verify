using System;

class ResolvedDiffTool
{
    public string Name { get; }
    public string ExePath { get; }
    public Func<FilePair, string> BuildArguments { get; }
    public bool IsMdi { get; }
    public bool SupportsAutoRefresh { get; }

    public string BuildCommand(FilePair filePair)
    {
        return $"\"{ExePath}\" {BuildArguments(filePair)}";
    }

    public ResolvedDiffTool(
        string name,
        string exePath,
        Func<FilePair, string> buildArguments,
        bool isMdi,
        bool supportsAutoRefresh)
    {
        Name = name;
        ExePath = exePath;
        BuildArguments = buildArguments;
        IsMdi = isMdi;
        SupportsAutoRefresh = supportsAutoRefresh;
    }
}