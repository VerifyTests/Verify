using System;
using System.IO;

static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;

        var uri = new UriBuilder(assembly.CodeBase!);
        var path = Uri.UnescapeDataString(uri.Path);

        CurrentDirectory = Path.GetDirectoryName(path)!;
    }

    public static string CurrentDirectory;
}