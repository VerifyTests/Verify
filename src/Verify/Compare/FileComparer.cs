using System;
using System.IO;
using System.Threading.Tasks;
using EmptyFiles;
using VerifyTests;

static class FileComparer
{
    public static async Task<EqualityResult> DoCompare(VerifySettings settings, FilePair file)
    {
        if (!File.Exists(file.Verified))
        {
            return Equality.MissingVerified;
        }

        if (AllFiles.IsEmptyFile(file.Verified))
        {
            return Equality.NotEqual;
        }

        var result = await FilesEqual(settings, file);
        if (result.IsEqual)
        {
            return Equality.Equal;
        }

        return new(Equality.NotEqual, result.Message);
    }

    static Task<CompareResult> FilesEqual(VerifySettings settings, FilePair filePair)
    {
        if (settings.TryFindComparer(out var compare))
        {
            return DoCompare(settings, compare!, filePair);
        }

        if (!FilesAreSameSize(filePair))
        {
            return Task.FromResult(CompareResult.NotEqual());
        }

        return DefaultCompare(settings, filePair);
    }

    public static Task<CompareResult> DefaultCompare(VerifySettings settings, FilePair filePair)
    {
        return DoCompare(
            settings,
            (stream1, stream2, _) => StreamsAreEqual(stream1, stream2),
            filePair);
    }

    static bool FilesAreSameSize(in FilePair file)
    {
        FileInfo first = new(file.Received);
        FileInfo second = new(file.Verified);
        return first.Length == second.Length;
    }

    static async Task<CompareResult> DoCompare(VerifySettings settings, Compare compare, FilePair filePair)
    {
#if NETSTANDARD2_0 || NETFRAMEWORK
        using var fs1 = FileHelpers.OpenRead(filePair.Received);
        using var fs2 = FileHelpers.OpenRead(filePair.Verified);
#else
        await using var fs1 = FileHelpers.OpenRead(filePair.Received);
        await using var fs2 = FileHelpers.OpenRead(filePair.Verified);
#endif
        return await compare(fs1, fs2, settings.Context);
    }

    #region DefualtCompare
    static async Task<CompareResult> StreamsAreEqual(Stream stream1, Stream stream2)
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
                return CompareResult.Equal;
            }

            for (var i = 0; i < count; i+= sizeof(long))
            {
                if (BitConverter.ToInt64(buffer1, i) != BitConverter.ToInt64(buffer2, i))
                {
                    return CompareResult.NotEqual();
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