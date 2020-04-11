using System.IO;

static class CodeBaseLocation
{
    static CodeBaseLocation()
    {
        var assembly = typeof(CodeBaseLocation).Assembly;

        var path = assembly.CodeBase
            .Replace("file:///", "")
            .Replace("file://", "")
            .Replace(@"file:\\\", "")
            .Replace(@"file:\\", "");

        CurrentDirectory = Path.GetDirectoryName(path);
    }

    public static string CurrentDirectory;
}