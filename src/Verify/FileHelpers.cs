﻿using System.IO;
using System.Text;
using System.Threading.Tasks;

static class FileHelpers
{
    public static readonly UTF8Encoding Utf8 = new(true, true);

    public static void DeleteIfEmpty(string path)
    {
        FileInfo fileInfo = new(path);
        if (fileInfo.Exists && fileInfo.Length == 0)
        {
            fileInfo.Delete();
        }
    }

    static FileStream OpenWrite(string filePath)
    {
        return new(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);
    }

    public static FileStream OpenRead(string path)
    {
        return new(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);
    }

    static Task<StringBuilder> ReadText(FileStream stream)
    {
        return stream.ReadAsStringBuilder();
    }

#if NETSTANDARD2_1 || NET5_0
    public static Task WriteText(string filePath, string text)
    {
        return File.WriteAllTextAsync(filePath, text, Utf8);
    }

    public static async Task<StringBuilder> ReadText(string filePath)
    {
        await using var stream = OpenRead(filePath);
        return await ReadText(stream);
    }

    public static async Task WriteStream(string filePath, Stream stream)
    {
        await using var fileStream = OpenWrite(filePath);
        await stream.CopyToAsync(fileStream);
    }
#else
    public static async Task WriteText(string filePath, string text)
    {
        var encodedText = Utf8.GetBytes(text);

        using var fileStream = OpenWrite(filePath);
        await fileStream.WriteAsync(encodedText, 0, encodedText.Length);
    }

    public static async Task WriteStream(string filePath, Stream stream)
    {
        using var fileStream = OpenWrite(filePath);
        await stream.CopyToAsync(fileStream);
    }

    public static async Task<StringBuilder> ReadText(string filePath)
    {
        using var stream = OpenRead(filePath);
        return await ReadText(stream);
    }
#endif
}