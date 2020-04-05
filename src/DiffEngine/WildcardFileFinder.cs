using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

static class WildcardFileFinder
{
    static char[] separators =
    {
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar
    };

    public static bool TryFind(string path, [NotNullWhen(true)] out string? result)
    {
        var expanded = Environment.ExpandEnvironmentVariables(path);
        if (path.Contains('*'))
        {
            var directoryPart = Path.GetDirectoryName(expanded);
            var filePart = Path.GetFileName(expanded);
            var segments = directoryPart.Split(separators);
            var currentPath = segments[0] + Path.DirectorySeparatorChar;
            foreach (var segment in segments.Skip(1))
            {
                if (segment.Contains('*'))
                {
                    currentPath = Directory.EnumerateDirectories(currentPath, segment)
                        .OrderByDescending(Directory.GetLastWriteTime)
                        .FirstOrDefault();
                    if (currentPath == null)
                    {
                        result = null;
                        return false;
                    }
                }
                else
                {
                    currentPath = Path.Combine(currentPath, segment);
                    if (!Directory.Exists(currentPath))
                    {
                        result = null;
                        return false;
                    }
                }
            }

            if (filePart.Contains('*'))
            {
                currentPath = Directory.EnumerateFiles(currentPath, filePart).FirstOrDefault();
                if (currentPath != null)
                {
                    result = currentPath;
                    return true;
                }
            }
            else
            {
                currentPath = Path.Combine(currentPath, filePart);
                if (File.Exists(currentPath))
                {
                    result = currentPath;
                    return true;
                }
            }

            result = null;
            return false;
        }

        if (File.Exists(expanded))
        {
            result = expanded;
            return true;
        }

        result = null;
        return false;
    }
}