using System.IO;

static partial class FileHelpers
{
    public static void DeleteIfEmpty(string path)
    {
        var fileInfo = new FileInfo(path);
        if (fileInfo.Exists && fileInfo.Length == 0)
        {
            fileInfo.Delete();
        }
    }

    public static void WriteEmptyText(string path)
    {
        File.CreateText(path).Dispose();
    }
}