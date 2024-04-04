interface IEnvironment
{
    string CurrentDirectory { get; }
    char DirectorySeparatorChar { get; }
    char AltDirectorySeparatorChar { get; }
    bool PathExists(string path);
    string CombinePaths(string path1, string path2);
}