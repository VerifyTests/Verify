static class FileHelpers
{
    public static readonly UTF8Encoding Utf8 = new(true, true);

    public static void DeleteIfEmpty(string path)
    {
        var info = new FileInfo(path);
        if (info.Exists && info.Length == 0)
        {
            info.Delete();
        }
    }

    static FileStream OpenWrite(string filePath)
    {
        return new(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.Read,
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

    public static long Length(string file)
    {
        return new FileInfo(file).Length;
    }

#if NETSTANDARD2_1 || NET5_0_OR_GREATER
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
        if (stream is FileStream fileStream)
        {
            File.Copy(fileStream.Name, filePath);
        }

        await using var targetStream = OpenWrite(filePath);
        await stream.CopyToAsync(targetStream);
    }
#else

    public static async Task WriteText(string filePath, string text)
    {
        var encodedText = Utf8.GetBytes(text);

        using var stream = OpenWrite(filePath);
        await stream.WriteAsync(encodedText, 0, encodedText.Length);
    }

    public static async Task WriteStream(string filePath, Stream stream)
    {
        if (stream is FileStream fileStream)
        {
            File.Copy(fileStream.Name, filePath);
            return;
        }

        using var targetStream = OpenWrite(filePath);
        await stream.CopyToAsync(targetStream);
    }

    public static async Task<StringBuilder> ReadText(string filePath)
    {
        using var stream = OpenRead(filePath);
        return await ReadText(stream);
    }
#endif
}