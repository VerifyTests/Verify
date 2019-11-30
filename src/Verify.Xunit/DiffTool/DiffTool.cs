class DiffTool
{
    public string Name { get; }
    public string Url { get; }
    public string ArgumentFormat { get; }
    public string[] BinaryExtensions { get; }
    public string[] ExePaths { get; }

    public DiffTool(string name, string url, string argumentFormat, string[] exePaths, string[] binaryExtensions)
    {
        Name = name;
        Url = url;
        ArgumentFormat = argumentFormat;
        BinaryExtensions = binaryExtensions;
        ExePaths = exePaths;
    }
}