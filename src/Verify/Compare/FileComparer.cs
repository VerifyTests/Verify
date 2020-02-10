﻿using System;
using System.IO;
using System.Threading.Tasks;
using Verify;

static class FileComparer
{
    public static async Task<CompareResult> DoCompare(VerifySettings settings, FilePair file)
    {
        if (!File.Exists(file.Verified))
        {
            return CompareResult.MissingVerified;
        }

        if (EmptyFiles.IsEmptyFile(file.Verified))
        {
            return CompareResult.NotEqual;
        }

        if (!await FilesEqual(settings, file))
        {
            return CompareResult.NotEqual;
        }

        return CompareResult.Equal;
    }

    public static Task<bool> FilesEqual(VerifySettings settings, FilePair file)
    {
        if (settings.comparer != null)
        {
            return DoCompare(file.Received, file.Verified, settings.comparer);
        }
        if (SharedVerifySettings.TryGetComparer(file.Extension, out var comparer))
        {
            return DoCompare(file.Received, file.Verified, comparer);
        }

        if (!FilesAreSameSize(file))
        {
            return Task.FromResult(false);
        }

        return DefaultCompare(file.Received, file.Verified);
    }

    public static Task<bool> DefaultCompare(string received, string verified)
    {
        return DoCompare(received, verified, StreamsAreEqual);
    }

    static bool FilesAreSameSize(FilePair file)
    {
        var first = new FileInfo(file.Received);
        var second = new FileInfo(file.Verified);
        return first.Length == second.Length;
    }

    static async Task<bool> DoCompare(string first, string second, Func<Stream, Stream, Task<bool>> compare)
    {
#if NETSTANDARD2_1
        await using var fs1 = FileHelpers.OpenRead(first);
        await using var fs2 = FileHelpers.OpenRead(second);
#else
        using var fs1 = FileHelpers.OpenRead(first);
        using var fs2 = FileHelpers.OpenRead(second);
#endif
        return await compare(fs1, fs2);
    }

    #region DefualtCompare
    static async Task<bool> StreamsAreEqual(Stream stream1, Stream stream2)
    {
        const int bufferSize = 1024 * sizeof(long);
        var buffer1 = new byte[bufferSize];
        var buffer2 = new byte[bufferSize];

        while (true)
        {
            var t1 = ReadBufferAsync(stream1, buffer1);
            await ReadBufferAsync(stream2, buffer2);

            var count = await t1;

            //no need to compare size here since only enter on files being same size

            if (count == 0)
            {
                return true;
            }

            for (var i = 0; i < count; i+= sizeof(long))
            {
                if (BitConverter.ToInt64(buffer1, i) != BitConverter.ToInt64(buffer2, i))
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
    #endregion
}