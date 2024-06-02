class PhysicalEnvironment : IEnvironment
{
    public static readonly IEnvironment Instance = new PhysicalEnvironment();

    PhysicalEnvironment()
    {
    }

    public string CurrentDirectory => Environment.CurrentDirectory;
    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;
    public bool PathExists(string path) => File.Exists(path) || Directory.Exists(path);
}