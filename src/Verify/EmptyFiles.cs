using System.Collections.Generic;
using System.IO;

static class EmptyFiles
{
    static Dictionary<string, EmptyFile> dictionary = new Dictionary<string, EmptyFile>();

    static EmptyFiles()
    {
        var emptyFiles = Path.Combine(CodeBaseLocation.CurrentDirectory, "Verify.EmptyFiles");
        foreach (var path in Directory.EnumerateFiles(emptyFiles, "*.*"))
        {
            var lastWriteTime = File.GetLastWriteTime(path);
            dictionary[FileHelpers.Extension(path)] = new EmptyFile(path, lastWriteTime);
        }
    }

    public static bool IsEmptyFile(string extension, string path)
    {
        if (!dictionary.TryGetValue(extension, out var emptyFile))
        {
            return false;
        }

        return File.GetLastWriteTime(path) == emptyFile.LastWriteTime;
    }

    public static bool TryWriteEmptyFile(string extension, string verifiedPath)
    {
        if (!dictionary.TryGetValue(extension, out var emptyFile))
        {
            return false;
        }

        File.Copy(emptyFile.Path, verifiedPath, true);
        return true;
    }
}