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

    static FileStream OpenWrite(string filePath)
    {
        return new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);
    }

    public static FileStream OpenRead(string path)
    {
        return new FileStream(path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);
    }
}