using System;

class ResolvedDiffTool
{
    public string Name { get; }
    public string ExePath { get; }
    public bool ShouldTerminate { get; }
    public Func<FilePair, string> BuildArguments { get; }
    public bool IsMdi { get; }
    public bool SupportsAutoRefresh { get; }

    public ResolvedDiffTool(
        string name,
        string exePath,
        bool shouldTerminate,
        Func<FilePair, string> buildArguments,
        bool isMdi,
        bool supportsAutoRefresh)
    {
        Name = name;
        ExePath = exePath;
        ShouldTerminate = shouldTerminate;
        BuildArguments = buildArguments;
        IsMdi = isMdi;
        SupportsAutoRefresh = supportsAutoRefresh;
    }
}