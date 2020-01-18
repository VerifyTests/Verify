class ResolvedDiffTool
{
    public string Name { get; }
    public string ExePath { get; }
    public string? ArgumentPrefix { get; }
    public bool ShouldTerminate { get; }

    public ResolvedDiffTool(string name, string exePath, string? argumentPrefix, bool shouldTerminate)
    {
        Name = name;
        ExePath = exePath;
        ArgumentPrefix = argumentPrefix;
        ShouldTerminate = shouldTerminate;
    }
}