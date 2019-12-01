using System;
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
            dictionary[Path.GetExtension(path).Substring(1)] = new EmptyFile(path, lastWriteTime);
        }
    }

}

class EmptyFile
{
    public string Path { get; }
    public DateTime LastWriteTime { get; }

    public EmptyFile(string path, in DateTime lastWriteTime)
    {
        Path = path;
        LastWriteTime = lastWriteTime;
    }
}