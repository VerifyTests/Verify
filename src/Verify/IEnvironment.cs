interface IEnvironment
{
    string CurrentDirectory { get; }
    char DirectorySeparatorChar { get; }
    bool PathExists(string path);
}