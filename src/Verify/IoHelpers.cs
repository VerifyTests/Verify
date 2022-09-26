﻿static class IoHelpers
{
    static readonly UTF8Encoding Utf8 = new(true, true);

    public static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public static void DeleteFileIfEmpty(string path)
    {
        var info = new FileInfo(path);
        if (info.Exists && info.Length == 0)
        {
            info.Delete();
        }
    }

    public static void DeleteFiles(string directory, string pattern)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        DeleteFiles(Files(directory, pattern));
    }

    public static List<string> Files(string directory, string pattern) =>
        Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories)
            .ToList();

    public static void DeleteFiles(IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            DeleteFiles(file);
        }
    }

    public static void DeleteFiles(string file)
    {
        try
        {
            File.Delete(file);
        }
        catch
        {
        }
    }

    public static void CreateDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static void MoveToStart(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
    }

    public static void ThrowIfEmpty(this Stream stream)
    {
        if (stream.Length == 0)
        {
            throw new("Empty data not supported.");
        }
    }

    static FileStream OpenWrite(string path) =>
        new(path, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize: 4096, useAsync: true);

    public static FileStream OpenRead(string path) =>
        new(path, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);

    public static long Length(string file) =>
        new FileInfo(file).Length;

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

    static async Task<StringBuilder> ReadStringBuilderWithFixedLines(this Stream stream)
    {
        var stringValue = await stream.ReadString();
        var builder = new StringBuilder(stringValue);
        builder.FixNewlines();
        return builder;
    }

    static bool TryCopyFileStream(string path, Stream stream)
    {
        if (stream is not FileStream fileStream)
        {
            return false;
        }

        File.Copy(fileStream.Name, path, true);
        return true;
    }


#if NET461 || NET472 || NET48 || NETSTANDARD2_0

    public static Task WriteText(string path, string text)
    {
        CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text, Utf8);
        return Task.CompletedTask;
    }

    public static Task WriteText(string path, StringBuilder text)
    {
        CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, text.ToString(), Utf8);
        return Task.CompletedTask;
    }

#else

    public static Task WriteText(string path, string text)
    {
        CreateDirectory(Path.GetDirectoryName(path)!);
        return File.WriteAllTextAsync(path, text, Utf8);
    }

    public static Task WriteText(string path, StringBuilder text)
    {
        CreateDirectory(Path.GetDirectoryName(path)!);
        return File.WriteAllTextAsync(path, text.ToString(), Utf8);
    }

#endif

    public static string GetRelativePath(string directory, string file)
    {
        var fullPath = Path.Combine(directory, file);
        if (directory.EndsWith(Path.DirectorySeparatorChar) ||
            directory.EndsWith(Path.AltDirectorySeparatorChar))
        {
            return fullPath[directory.Length..];
        }

        return fullPath[(directory.Length+1)..];

    }

#if NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER

    public static async Task<string> ReadStringWithFixedLines(string path)
    {
        await using var stream = OpenRead(path);
        return await stream.ReadStringWithFixedLines();
    }

    public static async Task<StringBuilder> ReadStringBuilderWithFixedLines(string path)
    {
        await using var stream = OpenRead(path);
        return await stream.ReadStringBuilderWithFixedLines();
    }

    public static async Task WriteStream(string path, Stream stream)
    {
        CreateDirectory(Path.GetDirectoryName(path)!);
        if (!TryCopyFileStream(path, stream))
        {
            await using var targetStream = OpenWrite(path);
            await stream.CopyToAsync(targetStream);
        }
    }

#else

    public static async Task<string> ReadStringWithFixedLines(string path)
    {
        using var stream = OpenRead(path);
        return await stream.ReadStringWithFixedLines();
    }

    public static async Task<StringBuilder> ReadStringBuilderWithFixedLines(string path)
    {
        using var stream = OpenRead(path);
        return await stream.ReadStringBuilderWithFixedLines();
    }

    public static async Task WriteStream(string path, Stream stream)
    {
        CreateDirectory(Path.GetDirectoryName(path));
        if (!TryCopyFileStream(path, stream))
        {
            using var targetStream = OpenWrite(path);
            await stream.CopyToAsync(targetStream);
        }
    }

#endif
}