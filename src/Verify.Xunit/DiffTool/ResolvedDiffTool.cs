class ResolvedDiffTool
{
    public string Name { get; }
    public string ExePath { get; }
    public string ArgumentFormat { get; }

    public ResolvedDiffTool(string name, string exePath, string argumentFormat)
    {
        Name = name;
        ExePath = exePath;
        ArgumentFormat = argumentFormat;
    }
}