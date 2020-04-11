using System;
using System.IO;

static class CodeBaseLocation
{
    static CodeBaseLocation()
    {
        var assembly = typeof(CodeBaseLocation).Assembly;

        var uri = new UriBuilder(assembly.CodeBase);
        var path = Uri.UnescapeDataString(uri.Path);

        CurrentDirectory = Path.GetDirectoryName(path);
    }

    public static string CurrentDirectory;
}