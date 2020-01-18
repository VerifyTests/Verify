using System;

class ResolvedDiffTool
{
    public string Name { get; }
    public string ExePath { get; }
    public bool ShouldTerminate { get; }
    public Func<FilePair, string> BuildArguments { get; }

    public ResolvedDiffTool(string name, string exePath, bool shouldTerminate, Func<FilePair, string> buildArguments)
    {
        Name = name;
        ExePath = exePath;
        ShouldTerminate = shouldTerminate;
        BuildArguments = buildArguments;
    }
}