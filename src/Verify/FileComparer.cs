using System;
using System.IO;
using System.Threading.Tasks;

static class FileComparer
{
    public static async Task<CompareResult> DoCompare(string receivedPath, string verifiedPath)
    {
        if (!File.Exists(verifiedPath))
        {
            return CompareResult.MissingVerified;
        }

        if (EmptyFiles.IsEmptyFile(verifiedPath))
        {
            return CompareResult.NotEqual;
        }

        if (!await FilesEqual(receivedPath, verifiedPath))
        {
            return CompareResult.NotEqual;
        }

        return CompareResult.Equal;
    }

    public static Task<bool> FilesEqual(string path1, string path2)
    {
        return FilesAreEqual(new FileInfo(path1), new FileInfo(path2));
    }

    static async Task<bool> FilesAreEqual(FileInfo first, FileInfo second)
    {
        if (first.Length != second.Length)
        {
            return false;
        }

        if (string.Equals(first.FullName, second.FullName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
#if NETSTANDARD2_1
        await using var fs1 = FileHelpers.OpenRead(first.FullName);
        await using var fs2 = FileHelpers.OpenRead(second.FullName);
        #else
        using var fs1 = FileHelpers.OpenRead(first.FullName);
        using var fs2 = FileHelpers.OpenRead(second.FullName);
#endif
        return await StreamsAreEqual(fs1, fs2);
    }

    static async Task<bool> StreamsAreEqual(Stream stream1, Stream stream2)
    {
        const int bufferSize = 1024 * sizeof(long);
        var buffer1 = new byte[bufferSize];
        var buffer2 = new byte[bufferSize];

        while (true)
        {
            var count1 = await ReadBufferAsync(stream1, buffer1);
            var count2 = await ReadBufferAsync(stream2, buffer2);

            if (count1 != count2)
            {
                return false;
            }

            if (count1 == 0)
            {
                return true;
            }

            var iterations = (int) Math.Ceiling((double) count1 / sizeof(long));
            for (var i = 0; i < iterations; i++)
            {
                var startIndex = i * sizeof(long);
                if (BitConverter.ToInt64(buffer1, startIndex) != BitConverter.ToInt64(buffer2, startIndex))
                {
                    return false;
                }
            }
        }
    }
    static async Task<int> ReadBufferAsync(Stream stream, byte[] buffer)
    {
        var bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
            if (read == 0)
            {
                // Reached end of stream.
                return bytesRead;
            }

            bytesRead += read;
        }

        return bytesRead;
    }
}