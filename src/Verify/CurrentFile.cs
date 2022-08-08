using IOPath = System.IO.Path;

namespace VerifyTests;

public static class CurrentFile
{
    public static string Path([CallerFilePath] string file = "") =>
        file;

    public static string Directory([CallerFilePath] string file = "") =>
        IOPath.GetDirectoryName(file)!;

    public static string Relative(string relative, [CallerFilePath] string file = "")
    {
        var directory = IOPath.GetDirectoryName(file)!;
        return IOPath.Combine(directory, relative);
    }
}