static class IoHelpers
{
    static readonly UTF8Encoding Utf8 = new(true, true);

    public static void DeleteIfEmpty(string path)
    {
        var info = new FileInfo(path);
        if (info.Exists && info.Length == 0)
        {
            info.Delete();
        }
    }

    public static void MoveToStart(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
    }

    static FileStream OpenWrite(string path)
    {
        return new(path, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize: 4096, useAsync: true);
    }

    public static FileStream OpenRead(string path)
    {
        return new(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
    }

    public static long Length(string file)
    {
        return new FileInfo(file).Length;
    }

    public static async Task<string> ReadString(this Stream stream)
    {
        stream.MoveToStart();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    static async Task<string> ReadStringWithFixedLines(this Stream stream)
    {
        var stringValue = await stream.ReadString();
        var builder = new StringBuilder(stringValue);
        builder.FixNewlines();
        return builder.ToString();
    }

#if NETSTANDARD2_1 || NET5_0_OR_GREATER

    public static Task WriteText(string path, string text)
    {
        return File.WriteAllTextAsync(path, text, Utf8);
    }

    public static async Task<string> ReadStringWithFixedLines(string path)
    {
        await using var stream = OpenRead(path);
        return await stream.ReadStringWithFixedLines();
    }

    public static async Task WriteStream(string path, Stream stream)
    {
        if (stream is FileStream fileStream)
        {
            File.Copy(fileStream.Name, path);
            return;
        }

        await using var targetStream = OpenWrite(path);
        await stream.CopyToAsync(targetStream);
    }

#else

    public static async Task WriteText(string path, string text)
    {
        var encodedText = Utf8.GetBytes(text);

        using var stream = OpenWrite(path);
        await stream.WriteAsync(encodedText, 0, encodedText.Length);
    }

    public static async Task WriteStream(string path, Stream stream)
    {
        if (stream is FileStream fileStream)
        {
            File.Copy(fileStream.Name, path, true);
            return;
        }

        using var targetStream = OpenWrite(path);
        await stream.CopyToAsync(targetStream);
    }

    public static async Task<string> ReadStringWithFixedLines(string path)
    {
        using var stream = OpenRead(path);
        return await stream.ReadStringWithFixedLines();
    }

#endif
}