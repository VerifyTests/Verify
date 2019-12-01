class ResolvedDiffTool
{
    public string Name { get; }
    public string ExePath { get; }
    public string ArgumentPrefix { get; }

    public ResolvedDiffTool(string name, string exePath, string argumentPrefix)
    {
        Name = name;
        ExePath = exePath;
        ArgumentPrefix = argumentPrefix;
    }
}