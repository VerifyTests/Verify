class DiffTool
{
    public string Name { get; }
    public string Url { get; }
    public string ArgumentPrefix { get; }
    public string[] BinaryExtensions { get; }
    public string[] ExePaths { get; }

    public DiffTool(string name, string url, string argumentPrefix, string[] exePaths, string[] binaryExtensions)
    {
        Name = name;
        Url = url;
        ArgumentPrefix = argumentPrefix;
        BinaryExtensions = binaryExtensions;
        ExePaths = exePaths;
    }
}