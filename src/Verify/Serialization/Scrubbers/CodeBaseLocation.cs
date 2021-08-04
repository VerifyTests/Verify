// ReSharper disable ConditionIsAlwaysTrueOrFalse
#if !NET5_0_OR_GREATER
using System;
using System.IO;

static class CodeBaseLocation
{
    static CodeBaseLocation()
    {
        var assembly = typeof(CodeBaseLocation).Assembly;

        if (assembly.CodeBase is not null)
        {
            UriBuilder uri = new(assembly.CodeBase);
            var path = Uri.UnescapeDataString(uri.Path);

            CurrentDirectory = Path.GetDirectoryName(path);
        }
    }

    public static string? CurrentDirectory;
}
#endif