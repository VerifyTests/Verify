using System;

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