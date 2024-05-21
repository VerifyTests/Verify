interface IEnvironment
{
    string CurrentDirectory { get; }
    char DirectorySeparatorChar { get; }
    char AltDirectorySeparatorChar { get; }
    bool PathExists(string path);
}