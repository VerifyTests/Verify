class PhysicalEnvironment : IEnvironment
{
    public static readonly IEnvironment Instance = new PhysicalEnvironment();

    PhysicalEnvironment()
    {
    }

    public string CurrentDirectory => Environment.CurrentDirectory;
    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;
    public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
    public bool PathExists(string path) => File.Exists(path) || Directory.Exists(path);
}